using System.Collections.Generic;

namespace Svn2GitMIgrator.Domain.Svn
{
    public interface ISvnService
    {
        IEnumerable<SvnRepoInfo> GetRepoList(SvnRepositoryRequest request);
        string Checkout(SvnRepositoryRequest request);
    }
}
