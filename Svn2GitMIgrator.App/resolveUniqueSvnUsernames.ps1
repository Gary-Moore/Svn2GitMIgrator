Param(
[Parameter(Mandatory=$true)][string]$path
)

svn log $path --quiet |
? { $_ -notlike '-*' } |
% { ($_ -split ' \| ')[1] } | 
Sort -Unique >  $path\authors-transform.txt