using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Svn2GitMIgrator.App.Models
{
    public class SvnRepoSearchViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string RootUrl { get; set; }
    }
}