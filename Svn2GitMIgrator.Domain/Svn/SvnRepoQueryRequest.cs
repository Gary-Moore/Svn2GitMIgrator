using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn2GitMIgrator.Domain.Svn
{
    public class SvnRepoQueryRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RootUrl { get; set; }

    }
}
