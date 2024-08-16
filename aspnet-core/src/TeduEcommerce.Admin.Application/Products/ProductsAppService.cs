﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeduEcommerce.ProductCategories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Products
{
    [Authorize]
    public class ProductsAppService : CrudAppService<Product, ProductDto, Guid, PagedResultRequestDto, CreateUpdateProductDto, CreateUpdateProductDto>, IProductsAppService
    {
        private readonly ProductManager _productManager;
        private readonly ProductCodeGenerator _productCodeGenerator;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        public ProductsAppService(IRepository<Product, Guid> repository, ProductManager productManager, IRepository<ProductCategory, Guid> productCategoryRepository, IBlobContainer<ProductThumbnailPictureContainer> fileContainer, ProductCodeGenerator productCodeGenerator) : base(repository)
        {
            _productManager = productManager;
            _productCategoryRepository = productCategoryRepository;
            _fileContainer = fileContainer;
            _productCodeGenerator = productCodeGenerator;
        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }

        public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));
            query = query.WhereIf(input.CategoryId.HasValue, i => i.CategoryId ==  input.CategoryId.Value);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductInListDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
        }

        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug, input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive, input.CategoryId, input.SeoMetaDescription, input.Description, input.SellPrice);

            if(input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }

            var result = await Repository.InsertAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(result);
        }

        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var product = await Repository.GetAsync(id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);

            product.ManufacturerId = input.ManufacturerId;
            product.Name = input.Name;
            product.Code = input.Code;
            product.Slug = input.Slug;
            product.ProductType = input.ProductType;
            product.SKU = input.SKU;
            product.SortOrder = input.SortOrder;
            product.Visibility = input.Visibility;
            product.IsActive = input.IsActive;

            if (product.CategoryId != input.CategoryId)
            {
                product.CategoryId = input.CategoryId;
                var category = await _productCategoryRepository.GetAsync(x => x.Id == input.CategoryId);
                product.CategoryName = category.Name;
                product.CategorySlug = category.Slug;
            }

            product.SeoMetaDescription = input.SeoMetaDescription;
            product.Description = input.Description;
            
            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }

            product.SellPrice = input.SellPrice;
            await Repository.UpdateAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        private async Task SaveThumnailImageAsync(string fileName, string base64)
        {
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] bytes = Convert.FromBase64String(base64);
            await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
        }

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            var thumnmailContent = await _fileContainer.GetAllBytesAsync(fileName);

            if(thumnmailContent == null)
                return null;

            var result = Convert.ToBase64String(thumnmailContent);
            return result;
        }

        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productCodeGenerator.GenerateAsync();
        }
    }
}
