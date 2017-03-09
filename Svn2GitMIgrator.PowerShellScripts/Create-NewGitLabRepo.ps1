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

    In this example, the script is simply run and the parameters are input as they are mandatory.
.EXAMPLE
   Migrate-SvnRepoToGit.ps1 -repoUrl http://svn-repo.com/myrepo/ -checkoutPath C:\temp\checkoutfolder
#>

#Create-NewGitLabRepo.ps1
#Gary Moore
#04 March, 2017
#Updated

##Paramaters
Param(
	[Parameter(Mandatory=$true)][string]$projectName,
	[Parameter(Mandatory=$true)][string]$path,
	[Parameter(Mandatory=$true)][string]$privatetoken,
	[Parameter(Mandatory=$true)][string]$gitlabUrl,
	[Parameter(Mandatory=$true)][string]$namespaceid
)

# Variables
$createProjectUrl = $gitlabUrl + "/v3/projects"

Write-Output "Creating new GitLab project"

# Create new project inside GitLab repo
$postParams = @{name=$projectName; namespace_id=$namespaceid; visibility="private"; path=$path; private_token=$privatetoken}
$response = Invoke-WebRequest -Uri $createProjectUrl -Method POST -Body $postParams

Write-Output $response

Write-Output "Project created"