using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Svn2GitMIgrator.Domain.FileSystem;
using Svn2GitMIgrator.Domain.Svn;
using Svn2GitMIgrator.Domain.TaskAutomation;

namespace Svn2GitMIgrator.Domain
{
    public class MigrationService : IMigrationService
    {
        private readonly ISvnService _svnService;
        private readonly string _workingDirectoryPath;

        public MigrationService(ISvnService svnService)
        {
            _svnService = svnService;
            _workingDirectoryPath = ConfigurationManager.AppSettings["WorkingFolderPath"];

            if (string.IsNullOrWhiteSpace(_workingDirectoryPath))
            {
                throw new ConfigurationErrorsException("App setting 'WorkingFolderPath' is missing or empty");
            }
        }

        public void Migrate(SvnRepositoryRequest request)
        {
            var checkoutPath = SetWorkingFolder(request.RepositorylUrl);

            // create new gitlab project
            var originUrl = "<gitlab url>";
            LogUniqueUserToFile(request, checkoutPath);

            CloneRepository(request, checkoutPath, originUrl);
            
            // Create new TeamCity Configuration
        }
        
        private void LogUniqueUserToFile(SvnRepositoryRequest request, string checkoutPath)
        {
            var filePath = FileSystemHelper.GetFilePath("knownUsernames.json");
            var knownUsers = JsonConvert.DeserializeObject<KnownUserList>(File.ReadAllText(filePath));
            var authorsFilePath = Path.Combine(checkoutPath, "authors-transform.txt");
            var uniqueAuthors = _svnService.LogUniqueUsers(request, checkoutPath).ToArray();

            StringBuilder authorText = new StringBuilder();
            foreach (var uniqueAuthor in uniqueAuthors)
            {
                var knownUser = knownUsers.Users.SingleOrDefault(x => x.Svnname == uniqueAuthor);

                if (knownUser != null)
                {
                    authorText.AppendFormat("{1} = {0} <{1}@parliament.uk>", knownUser.Author, knownUser.Svnname);
                    
                }
                else
                {
                    authorText.AppendFormat("{1} = {0} <{1}@parliament.uk>", "Mike Hunt", uniqueAuthor);
                }
                authorText.AppendLine();
            }
            
            File.WriteAllText(authorsFilePath, authorText.ToString());
        }

        private string SetWorkingFolder(string repositoryUrl)
        {
            var splitVals = repositoryUrl.Split('/');
            var appFolderName = splitVals[splitVals.Length - 2];
            var projectFolderName = splitVals[splitVals.Length - 3];

            var workingCheckoutDirectoryPath = Path.Combine(_workingDirectoryPath, projectFolderName, appFolderName);
            var directory = FileSystemHelper.EnsureFolderExists(workingCheckoutDirectoryPath);
            FileSystemHelper.ClearFolder(directory);

            return workingCheckoutDirectoryPath;
        }

        private void CloneRepository(SvnRepositoryRequest request, string checkoutPath, string originUrl)
        {
            var powerScript = new CloneSvnRepository();
            powerScript.AddArgument("repoUrl", request.RepositorylUrl);
            powerScript.AddArgument("checkoutPath", checkoutPath);
            powerScript.AddArgument("username", request.Username);
            powerScript.AddArgument("password", request.Password);

            powerScript.AddArgument("projectName", request.ProjectName);
            powerScript.AddArgument("privatetoken", request.PrivateToken);
            //powerScript.AddArgument("privatetoken", "iac_MyKnRhxXH1ZxWfEp");
            powerScript.AddArgument("gitlabUrl", request.GitLabUrl);
            //powerScript.AddArgument("gitlabUrl", "http://gitlab-devops-test.northeurope.cloudapp.azure.com");
            powerScript.AddArgument("originUrl", originUrl);
            //powerScript.AddArgument("originUrl", "http://gitlab-devops-test.northeurope.cloudapp.azure.com/garymoore/PapersLaid.Admin.git");
            
            powerScript.Execute();
        }
    }
}
