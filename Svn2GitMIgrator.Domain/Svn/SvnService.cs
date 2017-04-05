using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using SharpSvn;
using SharpSvn.Security;
using Svn2GitMIgrator.Domain.Git;

namespace Svn2GitMIgrator.Domain.Svn
{
    public class SvnService : ISvnService
    {
        private string _svnPassword;
        private string _svnRootUrl;
        private string _svnUsername;

        public SvnService()
        {
        }

        public IEnumerable<SvnRepoInfo> GetRepoList(SvnRepositoryRequest request)
        {
            SetCredentials(request);

            try
            {
                using (var client = GetSvnClient())
                {
                    Collection<SvnListEventArgs> contents;
                    var repoList = new List<SvnRepoInfo>();
                    if (client.GetList(new Uri(_svnRootUrl), out contents))
                    {
                        repoList = contents.Select(content => new SvnRepoInfo
                        {
                            Name = content.Name,
                            Url = content.Uri.AbsoluteUri
                        }).ToList();
                    }

                    return repoList;
                }
            }
            catch (SvnRepositoryIOException ex)
            {
                throw new SvnMigrationException(ex.Message, ex);
            }            
        }
        
        public IEnumerable<string> LogUniqueUsers(GitMigrationRequest request, string checkoutPath)
        {
            var authors = new List<string>();
            SetCredentials(request);
            try
            {
                using (var client = GetSvnClient())
                {
                    var repoUrl = SvnUriTarget.FromString(request.RepositorylUrl);
                    var args = GetSvnLogArgs();
                    
                    client.Log(repoUrl.Uri, args, (o, e) => {
                        authors.Add(e.Author);
                    });
                }
            }
            catch (SvnRepositoryIOException ex)
            {
                throw new SvnMigrationException(ex.Message, ex);
            }

            return authors.Distinct();
        }

        private static SvnLogArgs GetSvnLogArgs()
        {
            
            return new SvnLogArgs {Limit = 2000};
        }

        private void SetCredentials(SvnRepositoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Password))
            {
                throw new SvnMigrationException("A SVN password wasn't provided");
            }
            if (string.IsNullOrEmpty(request.RootUrl))
            {
                throw new SvnMigrationException("A SVN Repository Url wasn't provided");
            }
            if (string.IsNullOrEmpty(request.Username))
            {
                throw new SvnMigrationException("A SVN Username wasn't provided");
            }
            
            _svnPassword = request.Password;
            _svnRootUrl = request.RootUrl;
            _svnUsername = request.Username;
        }

        private void AuthenticateSvnClient(SvnClient client)
        {
            client.Authentication.Clear();
            client.Authentication.DefaultCredentials = new NetworkCredential(_svnUsername.Trim(), _svnPassword.Trim());
            client.Authentication.SslServerTrustHandlers += SVN_SSL_Override;
        }

        private static void ConfigureSvnClient(SvnClientContext svnClient)
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

        private static void SVN_SSL_Override(object sender, SvnSslServerTrustEventArgs e)
        {
            e.AcceptedFailures = e.Failures;
            e.Save = true;
        }
    }
}
