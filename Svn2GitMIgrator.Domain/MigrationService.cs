using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Svn2GitMIgrator.Domain.FileSystem;
using Svn2GitMIgrator.Domain.Svn;
using Svn2GitMIgrator.Domain.TaskAutomation;
using System;

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

        public void Migrate(GitMigrationRequest request)
        {
            var createGroupResult = CreateGitLabGroup(request);
            if (createGroupResult)
            {
                var groupId = createGroupResult.Outputs.FirstOrDefault().ToString();
                var createProjectResult = CreateGitLabProject(request, groupId);

                if (createProjectResult)
                {
                    var originUrl = ExtractGitOriginUrl(createProjectResult.Outputs.FirstOrDefault().ToString(), request.GitLabUrl);
                    var checkoutPath = SetWorkingFolder(request.RepositorylUrl);

                    // create new gitlab project
                    LogUniqueUserToFile(request, checkoutPath);

                    CloneRepository(request, checkoutPath, originUrl);
                }
            }
        }

        private string ExtractGitOriginUrl(string sourceUrl, string gitlabUrl)
        {
            var splitVals = sourceUrl.Split('/');
            var projectSegment = splitVals[splitVals.Length - 1];
            var groupSegment = splitVals[splitVals.Length - 2];

            return string.Format("{0}/{1}/{2}", gitlabUrl, groupSegment, projectSegment);
        }
        
        private void LogUniqueUserToFile(GitMigrationRequest request, string checkoutPath)
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

        private ScriptExecutionResult CreateGitLabProject(GitMigrationRequest request, string groupId)
        {
            var powerScript = new CreateGitLabProject();
            powerScript.AddArgument("projectname", request.GitProjectName);
            powerScript.AddArgument("gitlabUrl", request.GitLabUrl);
            powerScript.AddArgument("path", request.GitProjectPath);
            powerScript.AddArgument("namespaceid", groupId);
            powerScript.AddArgument("privatetoken", request.PrivateToken);

            return powerScript.Execute();
        } 

        private ScriptExecutionResult CreateGitLabGroup(GitMigrationRequest request)
        {
            var powerScript = new CreateGitLabGroup();
            powerScript.AddArgument("groupname", request.GitGroupName);
            powerScript.AddArgument("groupPath", request.GitGroupPath);
            powerScript.AddArgument("gitlabUrl", request.GitLabUrl);
            powerScript.AddArgument("privatetoken", request.PrivateToken);

            return powerScript.Execute();
        }

        private ScriptExecutionResult CloneRepository(GitMigrationRequest request, string checkoutPath, string originUrl)
        {
            var powerScript = new MigrateToGitRepository();
            powerScript.AddArgument("repoUrl", request.RepositorylUrl);
            powerScript.AddArgument("checkoutPath", checkoutPath);
            powerScript.AddArgument("username", request.Username);
            powerScript.AddArgument("password", request.Password);
            
            //powerScript.AddArgument("privatetoken", request.PrivateToken);
            powerScript.AddArgument("originUrl", originUrl);
            return powerScript.Execute();
        }        
    }
}
