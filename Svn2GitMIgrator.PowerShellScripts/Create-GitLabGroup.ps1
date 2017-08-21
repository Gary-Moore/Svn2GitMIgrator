#
# Create_GitLabGroup.ps1
#
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
	[Parameter(Mandatory=$true)][string]$groupName,
	[Parameter(Mandatory=$true)][string]$groupPath,
	[Parameter(Mandatory=$true)][string]$privatetoken,
	[Parameter(Mandatory=$true)][string]$gitlabUrl
)

# Variables
$groupsUrl = $gitlabUrl + "/api/v3/groups"
$getParams = @{search = $groupName; private_token=$privatetoken}

# Check if group already exists

$response = Invoke-RestMethod -Method Get -Uri $groupsUrl -Body $getParams

# if existing, return group id
if($response.name -eq $groupName){
	Write-Output $response.id
}
else{
	# else, create new group inside GitLab repo and return id
	$postParams = @{name=$groupName; visibility="public"; path=$groupPath; private_token=$privatetoken}
	$response = Invoke-RestMethod -Uri $groupsUrl -Method POST -Body $postParams

	Write-Output $response.id	
}
