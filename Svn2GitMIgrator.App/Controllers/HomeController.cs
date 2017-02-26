using DigiGitMigrator.Domain.Services;
using Svn2GitMIgrator.App.Models;
using Svn2GitMIgrator.Domain.Svn;
using System.Linq;
using System.Web.Mvc;

namespace Svn2GitMIgrator.App.Controllers
{
    public class HomeController : Controller
    {
        private ISvnService _svnService;

        public HomeController(ISvnService svnService)
        {
            _svnService = svnService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(SvnRepoQueryRequest request)
        {
            var data = _svnService.GetRepoList(request).ToList();
            return Json(data);
        }
    }
}