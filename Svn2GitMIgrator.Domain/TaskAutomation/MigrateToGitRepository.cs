namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class MigrateToGitRepository : PowershellScript
    {
        protected override string Name => "Migrate-SvnRepoToGit.ps1";
    }
}
    