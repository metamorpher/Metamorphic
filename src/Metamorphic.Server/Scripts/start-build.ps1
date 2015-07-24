[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string] $projectCollection,

    [ValidateNotNullOrEmpty()]
    [string] $project,

    [ValidateNotNullOrEmpty()]
    [string] $id,

    [ValidateNotNullOrEmpty()]
    [string] $user,

    [ValidateNotNullOrEmpty()]
    [string] $comment,

    [ValidateNotNullOrEmpty()]
    [string] $tfsServerUri = 'http://tfstest:8080/tfs'
)

Write-Verbose "Start-Build: param projectCollection = $projectCollection"
Write-Verbose "Start-Build: param project = $project"
Write-Verbose "Start-Build: param id = $id"
Write-Verbose "Start-Build: param user = $user"
Write-Verbose "Start-Build: param comment = $comment"
Write-Verbose "Start-Build: param tfsServerUri = $tfsServerUri"

$ErrorActionPreference = 'Stop'

$commonParameterSwitches =
    @{
        Verbose = $PSBoundParameters.ContainsKey('Verbose');
        Debug = $PSBoundParameters.ContainsKey('Debug');
        ErrorAction = "Stop"
    }

# Expecting something like: Development complete. Test builds: [Cinema, FilmProgramming, HeadOffice]
$buildAndBranchRegex = '^(?:Development\s*complete.)\s*(?:Test\s*builds:\s*\[)(.*?)(?:\])'

if (-not ($comment -match $versionNumberRegEx))
{
    throw "$comment does not match the expected build comment of 'Development complete. Test builds: [Build1, Build2, ..]'"
}

$builds = ([regex]::Replace($comment, $buildAndBranchRegex, '$1')).Split(',')

[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Client")
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Build.Client")

$tfs = [Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($tfsServerUri)
$buildserver = $tfs.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])

# Start builds
foreach($build in $builds)
{
    $buildDefinitionToRun = $buildServer.GetBuildDefinition($projectCollection, "$($build)_Dev")
    if ($buildDefinitionToRun -ne $null)
    {
        if (-not $buildDefinitionToRun.Enabled)
        {
            $buildDefinitionToRun.Enabled = $true
            $buildDefinitionToRun.Save()
        }

        $buildRequest = $buildDefinitionToRun.CreateBuildRequest()
        $buildRequest.RequestedFor = $user

        Write-Output "Queueing build for: $($buildDefinitionToRun.Name)"
        $queuedBuild = $buildServer.QueueBuild($buildRequest)
        while ($queuedBuild.Build -eq $null)
        {
            Start-Sleep -Seconds 1
            $queuedBuild.Refresh([Microsoft.TeamFoundation.Build.Client.QueryOptions]::BatchedRequests)
            Write-Output "Waiting for build to start running ..."
        }

        while (-not ($queuedBuild.Build.BuildNumber.StartsWith($buildDefinitionToRun.Name)))
        {
            Start-Sleep -Seconds 1
            $queuedBuild.Refresh([Microsoft.TeamFoundation.Build.Client.QueryOptions]::BatchedRequests)
            Write-Output "Waiting for build to get it's proper build ID ..."
        }

        # Can't provide the build with meta data so just write this crap to a file
        $content = "Requested by pipeline: Issue: $id"
        Set-Content -Path (Join-Path $PSScriptRoot $queuedBuild.Build.BuildNumber) -Value $content -Verbose -Force
    }
}