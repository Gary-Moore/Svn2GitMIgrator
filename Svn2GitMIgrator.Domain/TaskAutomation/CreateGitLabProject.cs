using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class CreateGitLabProject : PowershellScript
    {
        public CreateGitLabProject(Action<string> callback) : base(callback)
        {
        }

        protected override string Name => "Create-NewGitLabRepo.ps1";
    }
}
