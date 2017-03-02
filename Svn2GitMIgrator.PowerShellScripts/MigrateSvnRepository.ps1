#
# MigrateSvnRepository.ps1
#

Param(
[Parameter(Mandatory=$true)][string]$repoUrl, [Parameter(Mandatory=$true)][string]$checkoutPath, [Parameter(Mandatory=$true)][string]$username, [Parameter(Mandatory=$true)][string]$password
)

# clone 
echo $password | git svn clone $repoUrl --username $username --no-metadata -A $checkoutPath/authors-transform.txt --stdlayout  $checkoutPath/~/temp

# Download visual studio gitignore file for repository folder
Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile $checkoutPath/~/temp/.gitignore


$postParams = @{name='$projectName'; private_token='$privatetoken'}
Invoke-WebRequest -Uri $gitlabUrl -Method POST -Body $postParams
cd $checkoutPath/~/temp
git remote add origin $originUrl
git add .
git commit -m 'migration'
git push -u origin master