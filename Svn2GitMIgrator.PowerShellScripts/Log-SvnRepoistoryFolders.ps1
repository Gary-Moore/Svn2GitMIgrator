#
# Log_SvnRepoistoryFolders.ps1
#


[xml] $folders = svn list https://ukp.sliksvn.com/rapidapps/ --xml

Foreach($f in $folders.lists.list.entry){
    Write-Output $f.name
    $projectfoldername = 'https://ukp.sliksvn.com/rapidapps/' + $f.name
    $projectName = 'Project - ' + $f.name
    $subprojectfolder = svn ls $projectfoldername --xml

    Write-Output $subprojectfolder

    #$projectName, $subprojectfolder >> svn.xls
}