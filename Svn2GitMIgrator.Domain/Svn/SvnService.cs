using System;
using System.Collections.Generic;
using System.Net;
using System.Collections.ObjectModel;
using SharpSvn;
using SharpSvn.Security;
using System.Configuration;
using Svn2GitMIgrator.Domain.Svn;
using System.Linq;
using System.IO;

namespace DigiGitMigrator.Domain.Services
{
    public class SvnService : ISvnService
    {
        private string _svnPassword;
        private string _svnRootUrl;
        private string _svnUsername;

        public SvnService()
        {
        }

        public IEnumerable<SvnRepoInfo> GetRepoList(SvnRepoQueryRequest request)
        {
            _svnPassword = request.Password;
            _svnRootUrl = request.RootUrl;
            _svnUsername = request.Username;

            using (var client = GetSvnClient())
            {
                var contents = new Collection<SvnListEventArgs>();
                var repoList = new List<SvnRepoInfo>();
                if(client.GetList(new Uri(_svnRootUrl), out contents))
                {
                    repoList = contents.Where(content => !string.IsNullOrEmpty(content.Name) ).Select(content => new SvnRepoInfo
                    {
                        Name = content.Name,
                        Url = content.Uri.AbsoluteUri
                    }).ToList();
                }

                return repoList;
            }
        }

        private void AuthenticateSvnClient(SvnClient client)
        {
            client.Authentication.Clear();
            client.Authentication.DefaultCredentials = new NetworkCredential(_svnUsername.Trim(), _svnPassword.Trim());
            client.Authentication.SslServerTrustHandlers += new EventHandler<SvnSslServerTrustEventArgs>(SVN_SSL_Override);
        }

        private void ConfigureSvnClient(SvnClient svnClient)
        {
            var webAppDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var configPath = Path.Combine(webAppDirectoryInfo.Parent.FullName, "Svn");
            svnClient.LoadConfiguration(configPath, true);
        }

        private SvnClient GetSvnClient()
        {
            var client = new SvnClient();
            ConfigureSvnClient(client);
            AuthenticateSvnClient(client);
            return client;
        }

        static void SVN_SSL_Override(object sender, SvnSslServerTrustEventArgs e)
        {
            e.AcceptedFailures = e.Failures;
            e.Save = true;
        }
    }
}
