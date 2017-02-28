using Svn2GitMIgrator.App.Models;
using Svn2GitMIgrator.Domain.Svn;
using System.Linq;
using System.Web.Mvc;

namespace Svn2GitMIgrator.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISvnService _svnService;

        public HomeController(ISvnService svnService)
        {
            _svnService = svnService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MigrateRepo(SvnRepositoryRequest request)
        {
            _svnService.Checkout(request);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Search(SvnRepositoryRequest request)
        {
            var data = _svnService.GetRepoList(request).ToList();
            return Json(data);
        }
    }
}