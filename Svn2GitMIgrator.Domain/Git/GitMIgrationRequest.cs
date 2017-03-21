using Svn2GitMIgrator.Domain.Svn;

namespace Svn2GitMIgrator.Domain.Git
{
    public class GitMigrationRequest : SvnRepositoryRequest
    {
        public string GitLabUrl { get; set; }
        public string PrivateToken { get; set; }
        public string GitGroupName { get; set; }
        public string GitGroupPath { get; set; }
        public string GitProjectName { get; set; }
        public string GitProjectPath { get; set; }
        public string GitUserName { get; set; }
        public string GitUserEmail { get; set; }
        public string GitPassword { get; set; }
        public bool NonStandardFolder { get; set; }
    }
}
