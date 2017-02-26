using Svn2GitMIgrator.Domain.Svn;
using System.Collections.Generic;

namespace DigiGitMigrator.Domain.Services
{
    public interface ISvnService
    {
        IEnumerable<SvnRepoInfo> GetRepoList(SvnRepoQueryRequest request);
    }
}
