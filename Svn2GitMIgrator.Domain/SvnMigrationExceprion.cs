using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn2GitMIgrator.Domain
{
    public class SvnMigrationExceprion : Exception
    {
        public SvnMigrationExceprion(string message) : base(message)
        {

        }

        public SvnMigrationExceprion(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
