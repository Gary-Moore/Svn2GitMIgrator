using System.Collections.Generic;

namespace Svn2GitMIgrator.Domain
{
    public class MigrationResult
    {
        public MigrationResult()
        {
            ErrorMessages = new List<string>();
        }

        public bool Success{ get; set; }

        public ICollection<string> ErrorMessages { get; private set; }
    }
}
