<#
.Synopsis
    This is a script to migrate an SVN reposistory into a Git repository
.DESCRIPTION
   This is a script to migrate a legacy SVN reposistory into a new Git repository.
   All revision history including author names are maintained along with tags and branches.
.EXAMPLE
   Get-Support
   PS C:\scripts> .\Migrate-SvnRepoToGit.ps1

    cmdlet .\migrate-SvnRepoToGit.ps1 at command pipeline position 1
    Supply values for the following parameters:
    repoUrl: http://svn-repo.com/myrepo/
    checkoutPath: C:\temp\checkoutfolder
	password: Password123
	originUrl: http://GitLab-repo.com/myrepo/

    In this example, the script is simply run and the parameters are input as they are mandatory.
.EXAMPLE
   Migrate-SvnRepoToGit.ps1 -repoUrl http://svn-repo.com/myrepo/ -checkoutPath C:\temp\checkoutfolder -password password123 -originUrl http://GitLab.com/repo
#>

#Migrate-SvnRepoToGit.ps1
#Gary Moore
#04 March, 2017
#Updated

##Paramaters
Param(
	[Parameter(Mandatory=$true)][string]$repoUrl, 
	[Parameter(Mandatory=$true)][string]$checkoutPath,
	[Parameter(Mandatory=$true)][string]$username,
	[Parameter(Mandatory=$true)][string]$password,
	[Parameter(Mandatory=$true)][string]$originUrl,
	[Parameter(Mandatory=$true)][string]$gitUsername,
	[Parameter(Mandatory=$true)][string]$gitEmail
)

#Variables

Write-Progress "Cloning SVN repository into Git folder"

# Clone SVN repo into local Git Repository
echo $password | git svn clone $repoUrl --username $username --no-metadata --quiet -A $checkoutPath/authors-transform.txt -t releases/* -T trunk $checkoutPath/repo

# Download visual studio .gitignore file into repository folder from github
Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile $checkoutPath/repo/.gitignore

# Cleanup - convert git svn remote tags into real (lightweight) Git tags
cd $checkoutPath/repo
Foreach ($tag in git for-each-ref --format='%(refname:short)' refs/remotes/origin){
	# strip off excess tag paths, i.e. /origin/tags/1.0.0 becomes 1.0.0
    $tagName = [regex]::match($tag,'[^/\/]+$').Groups[0].Value 
	# create proper tag
	git tag $tagName $tag
	# remove remote tag
	git branch -D -r $tag
}

# add origin remote to GitLab central repo server 
git remote add origin $originUrl

git config user.email $gitEmail
git config user.name $gitUsername

# removed cached files an then add to aloow the .gitignore file to be applied (remove packages folder etc.)
git rm -r --cached .

# add all files, commit and push everything to origin
git add .
git commit -m 'migration to git'
git push origin master
git push origin master --tags