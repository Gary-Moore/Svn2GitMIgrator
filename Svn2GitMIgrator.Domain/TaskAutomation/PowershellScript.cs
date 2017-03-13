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
        private Action<string> _callback;
        protected readonly string ScriptFolderPath;

        protected PowershellScript(Action<string> callback)
        {
            ArgumentsList = new List<KeyValuePair<string, string>>();
            ScriptFolderPath = ConfigurationManager.AppSettings["PowerShellDirectory"];
            _callback = callback;
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
                        result.Outputs.Add(outputItem.BaseObject);
                    }
                }

                var errorStream = powershell.Streams.Error;
                if (errorStream.Count > 0)
                {
                    result.Suceess = false;
                    foreach (var error in errorStream)
                    {
                        result.ErrorMessages.Add(error.ToString());
                    }
                }
                else
                {
                    result.Suceess = true;
                }
            }

            return result;
        }

        public ScriptExecutionResult ExecuteAync()
        {
            var result = new ScriptExecutionResult();
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;
                PSCommand pscommand = new PSCommand();

                PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                outputCollection.DataAdded += outputCollection_DataAdded;
                powershell.Streams.Error.DataAdded += Error_DataAdded;

                var command = new Command(ResolveFilePath());
                // add the arguments
                foreach (var parameter in ArgumentsList)
                {
                    command.Parameters.Add(parameter.Key, parameter.Value);
                }

                powershell.Commands = pscommand.AddCommand(command);

                IAsyncResult ayncresult = powershell.BeginInvoke<PSObject, PSObject>(null, outputCollection);

                foreach (PSObject outputItem in outputCollection)
                {
                    if (outputItem != null)
                    {
                        result.Outputs.Add(outputItem.BaseObject);
                    }
                }
                
                var errorStream = powershell.Streams.Error;
                if (errorStream.Count > 0)
                {
                    result.Suceess = false;
                    foreach (var error in errorStream)
                    {
                        result.ErrorMessages.Add(error.ToString());
                    }
                }
                else
                {
                    result.Suceess = true;
                }
            }

            return result;
        }

        private void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> myp = (PSDataCollection<PSObject>)sender;
           
            Collection<PSObject> results = myp.ReadAll();
            foreach (PSObject result in results)
            {
                _callback(result.ToString());
            }
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<ErrorRecord> myp = (PSDataCollection<ErrorRecord>)sender;

            Collection<ErrorRecord> errors = myp.ReadAll();
            foreach (ErrorRecord error in errors) 
            {
                _callback(error.ToString());
            }
        }

    }
}

