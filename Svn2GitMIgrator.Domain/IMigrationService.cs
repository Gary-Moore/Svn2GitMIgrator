using Svn2GitMIgrator.Domain.Svn;

namespace Svn2GitMIgrator.Domain
{
    public interface IMigrationService
    {
        void Migrate(SvnRepositoryRequest request);
    }
}