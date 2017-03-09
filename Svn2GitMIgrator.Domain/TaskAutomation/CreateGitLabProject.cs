using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class CreateGitLabProject : PowershellScript
    {
        protected override string Name => "Create-NewGitLabRepo.ps1";
    }
}
