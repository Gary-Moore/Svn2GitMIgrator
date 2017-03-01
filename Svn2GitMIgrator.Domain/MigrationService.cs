using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Svn2GitMIgrator.Domain.FileSystem;
using Svn2GitMIgrator.Domain.Svn;

namespace Svn2GitMIgrator.Domain
{
    public class MigrationService : IMigrationService
    {
        private readonly ISvnService _svnService;

        public MigrationService(ISvnService svnService)
        {
            _svnService = svnService;
        }

        public void Migrate(SvnRepositoryRequest request)
        {
            var checkoutPath = _svnService.Checkout(request);
            
            LogUniqueUserToFile(request, checkoutPath);

            // clone directory into git

            // create git ignore file

            // push into a bare git repo

            // set default branch

            // push temp repo into new bare repo

            // delete temp repo

            // rename trunk to master

            // clean up branches

            // create new gitlab project

            // push to new project
        }
        
        private void LogUniqueUserToFile(SvnRepositoryRequest request, string checkoutPath)
        {
            var filePath = FileSystemHelper.GetFilePath("knownUsernames.json");

            var knownUsers = JsonConvert.DeserializeObject<KnownUserList>(File.ReadAllText(filePath));
            var authorsFilePath = Path.Combine(checkoutPath, "authors-transform.txt");
            var uniqueAuthors = _svnService.LogUniqueUsers(request, checkoutPath);

            StringBuilder authorText = new StringBuilder();
            foreach (var uniqueAuthor in uniqueAuthors)
            {
                var knownUser = knownUsers.Users.SingleOrDefault(x => x.Svnname == uniqueAuthor);

                if (knownUser != null)
                {
                    authorText.AppendFormat("{1} = {0} <{1}@parliament.uk>", knownUser.Author, knownUser.Svnname);
                    authorText.AppendLine();
                }                
            }
            
            File.WriteAllText(authorsFilePath, authorText.ToString());

        }
    }
}
