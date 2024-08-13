using Volo.Abp.Modularity;

namespace TeduEcommerce.Admin;

[DependsOn(
    typeof(TeduEcommerceAdminApplicationModule),
    typeof(TeduEcommerceDomainTestModule)
    )]
public class TeduEcommerceAdminApplicationTestModule : AbpModule
{

}
