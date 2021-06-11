using System.Web;
using System.Web.Mvc;

namespace Hybrid_Biz_Presentation_Tier
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
