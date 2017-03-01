using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public abstract class PowershellScript
    {
        protected abstract string Name { get; }
        private ICollection<KeyValuePair<string, string>> ArgumentsList { get; }
        protected const string ScriptFolderName = "PowerShellScripts";

        protected PowershellScript()
        {
            ArgumentsList = new List<KeyValuePair<string, string>>();
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
            var appDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (appDirectory.Parent != null)
            {
                return Path.Combine(appDirectory.Parent.FullName, ScriptFolderName, Name);
            }

            throw new InvalidOperationException("Failed to resolve the parent directory of the applicattion base directory");
        }

        public void Execute()
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;
                var command = Create();
                var pipeline = runspace.CreatePipeline();

                // add the arguments
                foreach (var parameter in ArgumentsList)
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
