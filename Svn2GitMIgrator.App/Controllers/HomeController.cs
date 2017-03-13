using Svn2GitMIgrator.Domain.Svn;
using System.Linq;
using System.Web.Mvc;
using Svn2GitMIgrator.Domain;
using Svn2GitMIgrator.App.Models;
using Microsoft.AspNet.SignalR;
using Svn2GitMIgrator.App.Hubs;
using System.Threading.Tasks;
using Svn2GitMIgrator.Domain.Git;

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
        public ActionResult MigrateRepo(GitMigrationRequest request)
        {
            WebResult result = new WebResult();
            try
            {
                _migrationService.Migrate(request, NotifyUpdates);
            }
            catch (SvnMigrationException ex)
            {
                result.Error = true;
                result.Message = ex.Message;
            }
            
            return Json(result);
        }

        [HttpPost]
        public ActionResult Search(SvnRepositoryRequest request)
        {
            WebResult result = new WebResult();
            try
            {
                var data = _svnService.GetRepoList(request).ToList();
                result.Data = data;
            }
            catch (SvnMigrationException ex)
            {
                result.Error = true;
                result.Message = ex.Message;
            }
            
            return Json(result);
        }

        public void NotifyUpdates(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MigrationHub>();
            if (hubContext != null)
            {
               hubContext.Clients.All.progress(message);
            }
        }
    }
}