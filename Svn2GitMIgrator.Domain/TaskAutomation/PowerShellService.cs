using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class PowerShellService
    {
        //private readonly ILog _log = LogManager.GetLogger(typeof(PowerShellService));

        public void RunScript(PSCommand command, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;
                // add the arguments
                foreach (var parameter in arguments)
                {
                    command.AddParameter(parameter.Key, parameter.Value);
                }

                powershell.Commands = command;

                Collection<PSObject> results = powershell.Invoke();

                foreach (var outputItem in results)
                {
                    if (outputItem != null)
                    {
                        
                    }
                }

                // log any errors
                var errorStream = powershell.Streams.Error;
                if (errorStream.Count > 0)
                {
                    foreach (var error in errorStream)
                    {
                        //error.ErrorDetails.Message;
                    }
                }
            }
        }
    }
}
