Param(
[Parameter(Mandatory=$true)][string]$repoUrl, [Parameter(Mandatory=$true)][string]$checkoutPath, [Parameter(Mandatory=$true)][string]$username, [Parameter(Mandatory=$true)][string]$password
)

Write-Output "Cloning SVN repository into Git folder"

echo $password | git svn clone $repoUrl --username $username --no-metadata -A $checkoutPath/authors-transform.txt --stdlayout  $checkoutPath/~/temp

git init $checkoutPath/~/new-bare.git
cd $checkoutPath/~/new-bare.git
git symbolic-ref HEAD refs/heads/trunk

cd $checkoutPath/~/temp
git remote add bare $checkoutPath/~/new-bare.git
git config remote.bare.push 'refs/remotes/*:refs/heads/*'
git push bare

cd $checkoutPath/~/new-bare.git
git branch -m trunk master

#cd ~/new-bare.git
#git for-each-ref --format='%(refname)' refs/heads/tags |
#cut -d / -f 4 |
#while read ref
#do
#  git tag "$ref" "refs/heads/tags/$ref";
#  git branch -D "tags/$ref";
#done

git remote add origin  http://gitlab-ce.hxakzvpf0otezeojz3wqhme5wg.cx.internal.cloudapp.net/garymoore/VetSurgery.git
git push --all origin
git push --tags origin