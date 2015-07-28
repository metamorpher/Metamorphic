[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string] $projectCollection,

    [ValidateNotNullOrEmpty()]
    [string] $project,

    [ValidateNotNullOrEmpty()]
    [string] $id,
    
    [ValidateNotNullOrEmpty()]
    [string] $comment,

    [ValidateNotNullOrEmpty()]
    [string] $tfsServerUri = 'http://tfstest:8080/tfs'
)

Write-Verbose "Invoke-Tests: param projectCollection = $projectCollection"
Write-Verbose "Invoke-Tests: param project = $project"
Write-Verbose "Invoke-Tests: param id = $id"
Write-Verbose "Invoke-Tests: param comment = $comment"

$ErrorActionPreference = 'Stop'

$commonParameterSwitches =
    @{
        Verbose = $PSBoundParameters.ContainsKey('Verbose');
        Debug = $PSBoundParameters.ContainsKey('Debug');
        ErrorAction = "Stop"
    }

[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.Client")
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.TeamFoundation.WorkItemTracking.Client")

$tfs = [Microsoft.TeamFoundation.Client.TeamFoundationServerFactory]::GetServer($tfsServerUri)
$store = $tfs.GetService([Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore]);

$workItem = $store.GetWorkItem($id)
$workItem.History = $comment
$workItem.Save()