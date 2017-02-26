using System.Web;
using System.Web.Mvc;

namespace Svn2GitMIgrator.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
