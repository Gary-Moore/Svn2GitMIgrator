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
	[Parameter(Mandatory=$true)][string]$gitUserName,
	[Parameter(Mandatory=$true)][string]$gitUserEmail,
	[Parameter(Mandatory=$true)][bool]$nonstandardfolder
)

#Variables
$workingRepoFolder = $checkoutPath + '/repo';

Write-Progress "Cloning SVN repository into Git folder"

# Clone SVN repo into local Git Repository
if($nonstandardfolder){
	# Non standard folder structure - non trunk sub folder
	echo $password | git svn clone $repoUrl.Trim('\') --username $username --no-metadata --quiet -A $checkoutPath/authors-transform.txt $workingRepoFolder
}else{
	# Standard folder structure with trunk and release folders
	echo $password | git svn clone $repoUrl --username $username --no-metadata --quiet -A $checkoutPath/authors-transform.txt -t releases/* -T trunk $workingRepoFolder
}


# Download visual studio .gitignore file into repository folder from github
Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile $workingRepoFolder/.gitignore

# Cleanup - convert git svn remote tags into real (lightweight) Git tags
cd $workingRepoFolder
Foreach ($tag in git for-each-ref --format='%(refname:short)' refs/remotes/origin){
	# strip off excess tag paths, i.e. /origin/tags/1.0.0 becomes 1.0.0
    $tagName = [regex]::match($tag,'[^/\/]+$').Groups[0].Value 
	# create proper tag
	git tag $tagName $tag
	# remove remote tag
	git branch -D -r $tag
}

git config user.name $gitUserName
git config user.email $gitUserEmail

# add origin remote to GitLab central repo server 
git remote add origin $originUrl


# removed cached files an then add to allow the .gitignore file to be applied (remove packages folder etc.)
git rm -r --cached .

# add all files, commit and push everything to origin
git add .
git commit -m 'migration to git'
git push origin master --quiet
git push origin master --tags --quiet

# When completed clean up the working directiory
Get-ChildItem ($checkoutPath + "\*") -Recurse | Remove-Item -Recurse -Force   