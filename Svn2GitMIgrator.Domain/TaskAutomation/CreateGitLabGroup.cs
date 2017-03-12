using System;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class CreateGitLabGroup : PowershellScript
    {
        public CreateGitLabGroup(Action<string> callback) : base(callback)
        {
        }

        protected override string Name => "Create-GitLabGroup.ps1";
    }
}
