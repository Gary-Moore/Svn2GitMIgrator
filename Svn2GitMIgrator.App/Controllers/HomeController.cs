using Svn2GitMIgrator.App.Models;
using Svn2GitMIgrator.Domain.Svn;
using System.Linq;
using System.Web.Mvc;
using Svn2GitMIgrator.Domain;

namespace Svn2GitMIgrator.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISvnService _svnService;
        private readonly IMigrationService _migrationService;

        public HomeController(ISvnService svnService, IMigrationService migrationService)
        {
            _svnService = svnService;
            _migrationService = migrationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MigrateRepo(SvnRepositoryRequest request)
        {
            _migrationService.Migrate(request);
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