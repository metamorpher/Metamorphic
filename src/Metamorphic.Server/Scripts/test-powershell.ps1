[CmdletBinding()]
param(
    [string] $text,
    [string] $logPath
)

$now = Get-Date
$dir = Join-Path $PSScriptRoot $logPath
$path = Join-Path ([System.IO.Path]::GetFullPath($dir)) "$($now.ToString('yyyy-MM-dd_HH-mm-ss')).txt"
Set-Content -Path $path -Value $text -Force -Verbose