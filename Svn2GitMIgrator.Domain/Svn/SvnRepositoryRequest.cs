namespace Svn2GitMIgrator.Domain.Svn
{
    public class SvnRepositoryRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RootUrl { get; set; }
        public string RepositorylUrl { get; set; }
    }
}
