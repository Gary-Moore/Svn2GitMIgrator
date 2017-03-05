using System.Collections.Generic;

namespace Svn2GitMIgrator.App.Models
{
    public class WebResult
    {
        public object Data { get; set; }

        public bool Error { get; set; }

        public string Message { get; set; }
    }
}