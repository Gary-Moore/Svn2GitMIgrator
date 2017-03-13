<#
.Synopsis
    This is a script to list all folders and first level subfolders within an SVN repository
.DESCRIPTION
   This is a script to list all folders and first level subfolders within an SVN repository.
   The names of each folder and subfolder will be written to excel spreadsheet.
.EXAMPLE
   Get-Support
   PS C:\scripts> .\List-SvnRepoFolders.ps1.ps1

    cmdlet .\List-SvnRepoFolders.ps1 at command pipeline position 1
    Supply values for the following parameters:
    svnUrl: http://svn-repo.com/myrepo/
    $outputFile: C:\temp\output.xls

    In this example, the script is simply run and the parameters are input as they are mandatory.
.EXAMPLE
   List-SvnRepoFolders.ps1 -svnUrl http://svn-repo.com/myrepo/ -outputFile C:\temp\output.xls 
#>

#List_SvnRepoFolders.ps1
#Gary Moore
#11 March, 2017
#Updated

##Paramaters
Param(
	[Parameter(Mandatory=$true)][string]$svnUrl,
	[Parameter(Mandatory=$true)][string]$outputFile
)

# Get all top level folders with the SVN repository
$directories =  [xml](svn ls $svnUrl --xml)

 Foreach ($directory in $directories.lists.list.entry){
    # output the name of each top level project folder to an excel file 
	$ProjectName = "*Project: " + $directory.name
    $ProjectName >> $outputFile
	# Get all next level child level folders with the parent project folder
    $projectpath = $svnUrl + $directory.name + '/'
    $subdirectories = [xml] (svn list $projectpath --xml)
	# Output the name of each child item folder
    Foreach($subdirectory in $subdirectories.lists.list.entry | Select-Object  -property @(@{N='kind';E={$_.GetAttribute('kind')}}, 'name') | Where-Object  { $_.kind -match ‘dir’}){
        $subdirectory.name >> $outputFile
    }       
}