namespace Svn2GitMIgrator.Domain.Svn
{
    public class SvnRepositoryRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RootUrl { get; set; }
        public string RepositorylUrl { get; set; }

        public string GitLabUrl { get; set; }
        public string PrivateToken { get; set; }
        public string NamespaceId { get; set; }
        public string GitProjectName { get; set; }
        public string GitProjectPath { get; set; }
    }
}
