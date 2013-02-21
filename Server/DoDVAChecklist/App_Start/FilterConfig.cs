using System.Web;
using System.Web.Mvc;
using DoDVAChecklist.Filters;

namespace DoDVAChecklist
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}