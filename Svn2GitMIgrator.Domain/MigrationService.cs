using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Svn2GitMIgrator.Domain.FileSystem;
using Svn2GitMIgrator.Domain.Svn;
using Svn2GitMIgrator.Domain.TaskAutomation;

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

            LogUniqueUserToFile(checkoutPath);

            ConvertUserNames(checkoutPath);

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

        private void LogUniqueUserToFile(string checkoutPath)
        {
            var powerScript = new ResolveUniqueSvnUserNames();
            powerScript.AddArgument("path", checkoutPath);
            powerScript.Execute();
        }

        private void ConvertUserNames(string checkoutPath)
        {
            var filePath = FileSystemHelper.GetFilePath("knownUsernames.json");

            var users = JsonConvert.DeserializeObject<KnownUserList>(File.ReadAllText(filePath));
            var authorsFilePath = Path.Combine(checkoutPath, "authors-transform.txt");


            string text = File.ReadAllText(authorsFilePath);
            foreach (var user in users.Users)
            {
                text = text.Replace(user.Svnname, string.Format("{0} <{1}@parliament.uk>", user.Author, user.Svnname));
            }

            File.WriteAllText(authorsFilePath, text);

        }
    }
}
