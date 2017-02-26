using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class PowerShellService
    {
        public void RunScript(string scriptFilePath, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;

                // add the script as the command
                var command = new PSCommand();
                command.AddCommand(scriptFilePath);

                // add the arguments
                foreach (var parameter in arguments)
                {
                    command.AddParameter(parameter.Key, parameter.Value);
                }

                powershell.Commands = command;

                Collection<PSObject> results = powershell.Invoke();
            }
        }
    }
}
