using Svn2GitMIgrator.Domain.Svn;
using System;
using Svn2GitMIgrator.Domain.Git;

namespace Svn2GitMIgrator.Domain
{
    public interface IMigrationService
    {
        void Migrate(GitMigrationRequest request, Action<string> callback);
    }
}