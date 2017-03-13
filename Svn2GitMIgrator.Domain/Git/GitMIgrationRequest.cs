namespace Svn2GitMIgrator.Domain.Svn
{
    public class GitMigrationRequest : SvnRepositoryRequest
    {
        public string GitLabUrl { get; set; }
        public string PrivateToken { get; set; }
        public string GitGroupName { get; set; }
        public string GitGroupPath { get; set; }
        public string GitProjectName { get; set; }
        public string GitProjectPath { get; set; }
    }
}
