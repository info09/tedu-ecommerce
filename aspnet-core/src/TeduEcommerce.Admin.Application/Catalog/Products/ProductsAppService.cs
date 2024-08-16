using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeduEcommerce.Attributes;
using TeduEcommerce.Catalog.Products.Attributes;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Catalog.Products
{
    [Authorize]
    public class ProductsAppService : CrudAppService<Product, ProductDto, Guid, PagedResultRequestDto, CreateUpdateProductDto, CreateUpdateProductDto>, IProductsAppService
    {
        private readonly ProductManager _productManager;
        private readonly ProductCodeGenerator _productCodeGenerator;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeDateTime> _attributeDateTimeRepository;
        private readonly IRepository<ProductAttributeDecimal> _attributeDecimalRepository;
        private readonly IRepository<ProductAttributeInt> _attributeIntRepository;
        private readonly IRepository<ProductAttributeText> _attributeTextRepository;
        private readonly IRepository<ProductAttributeVarchar> _attributeVarcharRepository;
        public ProductsAppService(IRepository<Product, Guid> repository,
                                    ProductManager productManager,
                                    IRepository<ProductCategory, Guid> productCategoryRepository,
                                    IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
                                    ProductCodeGenerator productCodeGenerator,
                                    IRepository<ProductAttribute> productAttributeRepository,
                                    IRepository<ProductAttributeDateTime> attributeDateTimeRepository,
                                    IRepository<ProductAttributeDecimal> attributeDecimalRepository,
                                    IRepository<ProductAttributeInt> attributeIntRepository,
                                    IRepository<ProductAttributeText> attributeTextRepository,
                                    IRepository<ProductAttributeVarchar> attributeVarcharRepository) : base(repository)
        {
            _productManager = productManager;
            _productCategoryRepository = productCategoryRepository;
            _fileContainer = fileContainer;
            _productCodeGenerator = productCodeGenerator;
            _attributeDateTimeRepository = attributeDateTimeRepository;
            _attributeDecimalRepository = attributeDecimalRepository;
            _attributeIntRepository = attributeIntRepository;
            _attributeTextRepository = attributeTextRepository;
            _attributeVarcharRepository = attributeVarcharRepository;
            _productAttributeRepository = productAttributeRepository;
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
            query = query.WhereIf(input.CategoryId.HasValue, i => i.CategoryId == input.CategoryId.Value);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductInListDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
        }

        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug, input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive, input.CategoryId, input.SeoMetaDescription, input.Description, input.SellPrice);

            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
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

            if (thumnmailContent == null)
                return null;

            var result = Convert.ToBase64String(thumnmailContent);
            return result;
        }

        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productCodeGenerator.GenerateAsync();
        }

        public async Task<ProductAttributeValueDto> AddProductAttributeAsync(AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(i => i.Id == input.AttributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

            var newAttributeId = Guid.NewGuid();
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (!input.DateTimeValue.HasValue)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDateTime = new ProductAttributeDateTime(newAttributeId, input.AttributeId, input.ProductId, input.DateTimeValue.Value);
                    await _attributeDateTimeRepository.InsertAsync(productAttributeDateTime);
                    break;
                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeVarchar = new ProductAttributeVarchar(newAttributeId, input.AttributeId, input.ProductId, input.VarcharValue);
                    await _attributeVarcharRepository.InsertAsync(productAttributeVarchar);
                    break;
                case AttributeType.Text:
                    if (input.TextValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeText = new ProductAttributeText(newAttributeId, input.AttributeId, input.ProductId, input.TextValue);
                    await _attributeTextRepository.InsertAsync(productAttributeText);
                    break;
                case AttributeType.Int:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeInt = new ProductAttributeInt(newAttributeId, input.AttributeId, input.ProductId, input.IntValue.Value);
                    await _attributeIntRepository.InsertAsync(productAttributeInt);
                    break;
                case AttributeType.Decimal:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDecimal = new ProductAttributeDecimal(newAttributeId, input.AttributeId, input.ProductId, input.DecimalValue.Value);
                    await _attributeDecimalRepository.InsertAsync(productAttributeDecimal);
                    break;
                default:
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = newAttributeId,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }

        public async Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);
            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (input.DateTimeValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDateTime = await _attributeDateTimeRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    productAttributeDateTime.Value = input.DateTimeValue.Value;
                    await _attributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                    break;
                case AttributeType.Int:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeInt = await _attributeIntRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    productAttributeInt.Value = input.IntValue.Value;
                    await _attributeIntRepository.UpdateAsync(productAttributeInt);
                    break;
                case AttributeType.Decimal:
                    if (input.DecimalValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDecimal = await _attributeDecimalRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    productAttributeDecimal.Value = input.DecimalValue.Value;
                    await _attributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                    break;
                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeVarchar = await _attributeVarcharRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    productAttributeVarchar.Value = input.VarcharValue;
                    await _attributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                    break;
                case AttributeType.Text:
                    if (input.TextValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeText = await _attributeTextRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    productAttributeText.Value = input.TextValue;
                    await _attributeTextRepository.UpdateAsync(productAttributeText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = id,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }

        public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
        {
            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == attributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    var productAttributeDateTime = await _attributeDateTimeRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _attributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                    break;
                case AttributeType.Int:

                    var productAttributeInt = await _attributeIntRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _attributeIntRepository.DeleteAsync(productAttributeInt);
                    break;
                case AttributeType.Decimal:
                    var productAttributeDecimal = await _attributeDecimalRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _attributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                    break;
                case AttributeType.Varchar:
                    var productAttributeVarchar = await _attributeVarcharRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _attributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                    break;
                case AttributeType.Text:
                    var productAttributeText = await _attributeTextRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _attributeTextRepository.DeleteAsync(productAttributeText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();
            var attributeQueryDateTime = await _attributeDateTimeRepository.GetQueryableAsync();
            var attributeQueryInt = await _attributeIntRepository.GetQueryableAsync();
            var attributeQueryDecimal = await _attributeDecimalRepository.GetQueryableAsync();
            var attributeQueryText = await _attributeTextRepository.GetQueryableAsync();
            var attributeQueryVarchar = await _attributeVarcharRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeQueryDateTime on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeQueryDecimal on a.Id equals adecimal.AttributeId into adecimalTable
                        from adecimal in adecimalTable.DefaultIfEmpty()
                        join aint in attributeQueryInt on a.Id equals aint.AttributeId into aintTable
                        from aint in aintTable.DefaultIfEmpty()
                        join atext in attributeQueryText on a.Id equals atext.AttributeId into atextTable
                        from atext in atextTable.DefaultIfEmpty()
                        join avarchar in attributeQueryVarchar on a.Id equals avarchar.AttributeId into avarcharTable
                        from avarchar in avarcharTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == productId)
                        && (adecimal == null || adecimal.ProductId == productId)
                         && (aint == null || aint.ProductId == productId)
                          && (avarchar == null || avarchar.ProductId == productId)
                           && (atext == null || atext.ProductId == productId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = productId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = atext != null ? atext.Value : null,
                            VarcharValue = avarchar != null ? avarchar.Value : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = atext != null ? atext.Id : null,
                            VarcharId = avarchar != null ? avarchar.Id : null,
                            DateTimeId = adate != null ? adate.Id : null,
                        };
            query = query.Where(x => x.DateTimeId != null
                           || x.DecimalId != null
                           || x.IntValue != null
                           || x.TextId != null
                           || x.VarcharId != null);
            return await AsyncExecuter.ToListAsync(query);
        }

        public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();
            var attributeQueryDateTime = await _attributeDateTimeRepository.GetQueryableAsync();
            var attributeQueryInt = await _attributeIntRepository.GetQueryableAsync();
            var attributeQueryDecimal = await _attributeDecimalRepository.GetQueryableAsync();
            var attributeQueryText = await _attributeTextRepository.GetQueryableAsync();
            var attributeQueryVarchar = await _attributeVarcharRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeQueryDateTime on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeQueryDecimal on a.Id equals adecimal.AttributeId into adecimalTable
                        from adecimal in adecimalTable.DefaultIfEmpty()
                        join aint in attributeQueryInt on a.Id equals aint.AttributeId into aintTable
                        from aint in aintTable.DefaultIfEmpty()
                        join atext in attributeQueryText on a.Id equals atext.AttributeId into atextTable
                        from atext in atextTable.DefaultIfEmpty()
                        join avarchar in attributeQueryVarchar on a.Id equals avarchar.AttributeId into avarcharTable
                        from avarchar in avarcharTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == input.ProductId)
                        && (adecimal == null || adecimal.ProductId == input.ProductId)
                         && (aint == null || aint.ProductId == input.ProductId)
                          && (avarchar == null || avarchar.ProductId == input.ProductId)
                           && (atext == null || atext.ProductId == input.ProductId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = input.ProductId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = atext != null ? atext.Value : null,
                            VarcharValue = avarchar != null ? avarchar.Value : null,
                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = atext != null ? atext.Id : null,
                            VarcharId = avarchar != null ? avarchar.Id : null,
                        };
            query = query.Where(x => x.DateTimeId != null
            || x.DecimalId != null
            || x.IntValue != null
            || x.TextId != null
            || x.VarcharId != null);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.Label)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                );
            return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
        }
    }
}
