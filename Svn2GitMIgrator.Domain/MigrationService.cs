using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Svn2GitMIgrator.Domain.FileSystem;
using Svn2GitMIgrator.Domain.Svn;
using Svn2GitMIgrator.Domain.TaskAutomation;
using System;
using Svn2GitMIgrator.Domain.Git;

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

        public void Migrate(GitMigrationRequest request, Action<string> callback)
        {
            callback("Creating group: " + request.GitGroupName + Environment.NewLine);
            var createGroupResult = CreateGitLabGroup(request, callback);
            if (createGroupResult)
            {
                callback("Creating project: " + request.GitProjectName + Environment.NewLine);
                var groupId = createGroupResult.Outputs.FirstOrDefault().ToString();
                var createProjectResult = CreateGitLabProject(request, groupId, callback);

                if (createProjectResult)
                {
                    var gitLabProjectUrl = createProjectResult.Outputs.FirstOrDefault().ToString();
                    var originUrl = ExtractGitOriginUrl(gitLabProjectUrl, request.GitLabUrl, request.GitUserName, request.GitPassword);
                    callback("Created origin at : " + gitLabProjectUrl + Environment.NewLine);

                    var checkoutPath = SetWorkingFolder(request.RepositorylUrl);

                    // create new gitlab project
                    LogUniqueUserToFile(request, checkoutPath);
                    callback("Migrating repository" + Environment.NewLine);
                    var cloneResult = CloneRepository(request, checkoutPath, originUrl, callback);

                    if (cloneResult)
                    {
                        callback("SVN Repository successfully migrated!" + Environment.NewLine);
                    }
                }
                else
                {
                    callback("Error creating project:" + Environment.NewLine);
                    OutputErrors(createProjectResult, callback);
                }
            }
            else
            {
                callback("Error creating group:" + Environment.NewLine);
                OutputErrors(createGroupResult, callback);
            }
        }

        private void OutputErrors(ScriptExecutionResult result, Action<string> errorCallback)
        {
            foreach (var error in result.ErrorMessages)
            {
                errorCallback(error + Environment.NewLine);
            }
        }

        private string ExtractGitOriginUrl(string sourceUrl, string gitlabUrl, string username, string password)
        {
            var splitVals = sourceUrl.Split('/');
            var projectSegment = splitVals[splitVals.Length - 1];
            var groupSegment = splitVals[splitVals.Length - 2];
            var host = new Uri(gitlabUrl).Host;

            return $"http://{username}:{password}@{host}/{groupSegment}/{projectSegment}";
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
                    authorText.AppendFormat("{0} = {0} <{0}@parliament.uk>", uniqueAuthor);
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

        private ScriptExecutionResult CreateGitLabProject(GitMigrationRequest request, string groupId, Action<string> callback)
        {
            var powerScript = new CreateGitLabProject(callback);
            powerScript.AddArgument("projectname", request.GitProjectName);
            powerScript.AddArgument("gitlabUrl", request.GitLabUrl);
            powerScript.AddArgument("path", request.GitProjectPath);
            powerScript.AddArgument("namespaceid", groupId);
            powerScript.AddArgument("privatetoken", request.PrivateToken);

            return powerScript.Execute();
        } 

        private ScriptExecutionResult CreateGitLabGroup(GitMigrationRequest request, Action<string> callback)
        {
            var powerScript = new CreateGitLabGroup(callback);
            powerScript.AddArgument("groupname", request.GitGroupName);
            powerScript.AddArgument("groupPath", request.GitGroupPath);
            powerScript.AddArgument("gitlabUrl", request.GitLabUrl);
            powerScript.AddArgument("privatetoken", request.PrivateToken);

            return powerScript.Execute();
        }

        private ScriptExecutionResult CloneRepository(GitMigrationRequest request, string checkoutPath, string originUrl, Action<string> callback)
        {
            var powerScript = new MigrateToGitRepository(callback);
            powerScript.AddArgument("repoUrl", request.RepositorylUrl);
            powerScript.AddArgument("checkoutPath", checkoutPath);
            powerScript.AddArgument("username", request.Username);
            powerScript.AddArgument("password", request.Password);
            powerScript.AddArgument("gitUserEmail", request.GitUserEmail);
            powerScript.AddArgument("gitUserName", request.GitUserName);
            powerScript.AddArgument("originUrl", originUrl);

            return powerScript.ExecuteAync();
        }        
    }
}
