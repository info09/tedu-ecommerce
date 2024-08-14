using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.ProductCategories
{
    public class ProductCategoriesAppService : CrudAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        private readonly IRepository<ProductCategory, Guid> _repository;
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await _repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductCategoryInListDto>(totalCount, ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data));
        }
    }
}
