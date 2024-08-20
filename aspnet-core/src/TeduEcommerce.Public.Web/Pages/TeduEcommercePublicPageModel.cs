using TeduEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace TeduEcommerce.Public.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class TeduEcommercePublicPageModel : AbpPageModel
{
    protected TeduEcommercePublicPageModel()
    {
        LocalizationResourceType = typeof(TeduEcommerceResource);
    }
}
