using System.Collections.Generic;
using Svn2GitMIgrator.Domain.Git;

namespace Svn2GitMIgrator.Domain.Svn
{
    public interface ISvnService
    {
        IEnumerable<SvnRepoInfo> GetRepoList(SvnRepositoryRequest request);
        IEnumerable<string> LogUniqueUsers(GitMigrationRequest request, string checkoutPath);
    }
}
