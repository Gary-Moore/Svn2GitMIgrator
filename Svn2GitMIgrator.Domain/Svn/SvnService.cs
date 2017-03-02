using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using SharpSvn;
using SharpSvn.Security;
using Svn2GitMIgrator.Domain.FileSystem;

namespace Svn2GitMIgrator.Domain.Svn
{
    public class SvnService : ISvnService
    {
        private string _svnPassword;
        private string _svnRootUrl;
        private string _svnUsername;
        private readonly string _workingDirectoryPath;

        public SvnService()
        {
            _workingDirectoryPath = ConfigurationManager.AppSettings["WorkingFolderPath"];

            if (string.IsNullOrWhiteSpace(_workingDirectoryPath))
            {
                throw new ConfigurationErrorsException("App setting 'WorkingFolderPath' is missing or empty");
            }
        }

        public IEnumerable<SvnRepoInfo> GetRepoList(SvnRepositoryRequest request)
        {
            SetCredentials(request);

            using (var client = GetSvnClient())
            {
                Collection<SvnListEventArgs> contents;
                var repoList = new List<SvnRepoInfo>();
                if(client.GetList(new Uri(_svnRootUrl), out contents))
                {
                    repoList = contents.Where(content => !string.IsNullOrEmpty(content.Name) )
                        .Select(content => new SvnRepoInfo
                    {
                        Name = content.Name,
                        Url = content.Uri.AbsoluteUri
                    }).ToList();
                }

                return repoList;
            }
        }

        public string Checkout(SvnRepositoryRequest request)
        {
            SetCredentials(request);
            var workingCheckoutDirectoryPath = SetWorkingCheckoutDirectoryPath(request.RepositorylUrl);

            using (var client = GetSvnClient())
            {
                var repoUrl = SvnUriTarget.FromString(request.RepositorylUrl);
                client.CheckOut(repoUrl, workingCheckoutDirectoryPath);
                client.Upgrade(workingCheckoutDirectoryPath);
            }

            return workingCheckoutDirectoryPath;
        }

        public IEnumerable<string> LogUniqueUsers(SvnRepositoryRequest request, string checkoutPath)
        {
            var authors = new List<string>();
            SetCredentials(request);
            using (var client = GetSvnClient())
            {
                var repoUrl = SvnUriTarget.FromString(request.RepositorylUrl);
                client.Log(repoUrl.Uri, (o, e) => {
                    authors.Add(e.Author);
                });
            }

            return authors.Distinct();
        }

        private string SetWorkingCheckoutDirectoryPath(string repositorylUrl)
        {
            var splitVals = repositorylUrl.Split('/');
            var appFolderName = splitVals[splitVals.Length - 2];

            var workingCheckoutDirectoryPath = Path.Combine(_workingDirectoryPath, splitVals[splitVals.Length - 3], splitVals[splitVals.Length - 2]);
            var directory = FileSystemHelper.EnsureFolderExists(workingCheckoutDirectoryPath);
            FileSystemHelper.ClearFolder(directory);
            
            return workingCheckoutDirectoryPath;
        }

        private void SetCredentials(SvnRepositoryRequest request)
        {
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
