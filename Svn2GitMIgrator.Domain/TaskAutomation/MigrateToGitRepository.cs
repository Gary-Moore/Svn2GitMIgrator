using System;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class MigrateToGitRepository : PowershellScript
    {
        public MigrateToGitRepository(Action<string> callback) : base(callback)
        {
        }

        protected override string Name => "Migrate-SvnRepository.ps1";
    }
}
    