using System;

namespace Svn2GitMIgrator.Domain
{
    public class SvnMigrationException : Exception
    {
        public SvnMigrationException(string message) : base(message)
        {

        }

        public SvnMigrationException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
