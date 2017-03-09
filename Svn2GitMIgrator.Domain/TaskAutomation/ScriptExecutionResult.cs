using System.Collections.Generic;

namespace Svn2GitMIgrator.Domain.TaskAutomation
{
    public class ScriptExecutionResult
    {
        public ScriptExecutionResult()
        {
            Messages = new List<string>();
            ErrorMessages = new List<string>();
        }

        public bool Suceess { get; set; }

        public IList<string> Messages { get; private set; }
        public IList<string> ErrorMessages { get; private set; }
    }
}
