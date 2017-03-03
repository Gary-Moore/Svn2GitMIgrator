#
# MigrateSvnRepository.ps1
#

Param(
	[Parameter(Mandatory=$true)][string]$repoUrl, 
	[Parameter(Mandatory=$true)][string]$checkoutPath,
	[Parameter(Mandatory=$true)][string]$username, 
	[Parameter(Mandatory=$true)][string]$password,
	[Parameter(Mandatory=$true)][string]$projectName,
	[Parameter(Mandatory=$true)][string]$privatetoken,
	[Parameter(Mandatory=$true)][string]$gitlabUrl,
	[Parameter(Mandatory=$true)][string]$originUrl
)

Write-Output "Cloning SVN repository into Git folder"

echo $password | git svn clone $repoUrl --username $username --no-metadata -A $checkoutPath/authors-transform.txt -t tags -b branches\releases -T trunk  $checkoutPath/repo

# clone 
#echo $password | git svn clone $repoUrl --username $username --no-metadata -A $checkoutPath/authors-transform.txt -t tags -b releases -T trunk  $checkoutPath/repo

# Download visual studio gitignore file for repository folder
#Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile $checkoutPath/repo/.gitignore


#$postParams = @{name=$projectName; private_token=$privatetoken; visibility_level=10}
#Invoke-WebRequest -Uri $gitlabUrl -Method POST -Body $postParams
cd $checkoutPath/repo
Write-Output $originUrl
#git remote add origin $originUrl
#git add .
#git commit -m 'migration'
#git push -u origin master

#-b branches/releases