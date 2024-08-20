using AutoMapper;
using TeduEcommerce.Attributes;
using TeduEcommerce.Manufacturers;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;
using TeduEcommerce.Public.Catalog.Manufacturers;
using TeduEcommerce.Public.Catalog.ProductAttributes;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public;

public class TeduEcommercePublicApplicationAutoMapperProfile : Profile
{
    public TeduEcommercePublicApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();

        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
    }
}
