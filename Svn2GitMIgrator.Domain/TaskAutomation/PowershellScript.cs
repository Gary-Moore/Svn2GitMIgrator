using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public abstract class PowershellScript
    {
        protected abstract string Name { get; }
        private ICollection<KeyValuePair<string, string>> ArgumentsList { get; }

        protected readonly string ScriptFolderPath;

        public List<string> ErrorMessages { get; }

        protected PowershellScript()
        {
            ArgumentsList = new List<KeyValuePair<string, string>>();
            ScriptFolderPath = ConfigurationManager.AppSettings["PowerShellDirectory"];
            ErrorMessages = new List<string>();
        }

        public PSCommand Create()
        {
            var scriptFilePath = ResolveFilePath();

            var command = new PSCommand();
            command.AddScript(scriptFilePath);

            return command;
        }

        public void AddArgument(string key, string value)
        {
            ArgumentsList.Add(new KeyValuePair<string, string>(key, value));
        }

        protected string ResolveFilePath()
        {
            return Path.Combine(ScriptFolderPath, Name);
        }

        public ScriptExecutionResult Execute()
        {
            var result = new ScriptExecutionResult();
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;
                PSCommand pscommand = new PSCommand();

                var command = new Command(ResolveFilePath());
                // add the arguments
                foreach (var parameter in ArgumentsList)
                {
                    command.Parameters.Add(parameter.Key, parameter.Value);
                }

                powershell.Commands = pscommand.AddCommand(command);

                Collection<PSObject> results = powershell.Invoke();

                foreach (var outputItem in results)
                {
                    if (outputItem != null)
                    {
                        result.Messages.Add(outputItem.BaseObject.ToString());
                    }
                }
                
                var errorStream = powershell.Streams.Error;
                if (errorStream.Count > 0)
                {
                    result.Suceess = false;
                    foreach (var error in errorStream)
                    {
                        result.Messages.Add(error.ToString());
                    }
                }
                else
                {
                    result.Suceess = true;
                }
            }

            return result;
        }
    }
}
