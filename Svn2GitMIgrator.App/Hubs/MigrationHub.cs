using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Svn2GitMIgrator.App.Hubs
{
    public class MigrationHub : Hub
    {
        public void ProgressUpdate(string message)
        {
            Clients.All.progress(message);
        }
    }
}