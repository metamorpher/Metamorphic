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
    [string] $tfsServerUri = 'http://tfstest:8080/tfs'
)

Write-Verbose "Update-IssueStatus: param projectCollection = $projectCollection"
Write-Verbose "Update-IssueStatus: param project = $project"
Write-Verbose "Update-IssueStatus: param id = $id"
Write-Verbose "Update-IssueStatus: param user = $user"
Write-Verbose "Update-IssueStatus: param tfsServerUri = $tfsServerUri"

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

$parentWorkItemId = -1
foreach($linkedItem in $workItem.Links)
{
    if ($linkedItem.LinkTypeEnd.Name -eq 'Parent')
    {
        $parentWorkItemId = $linkedItem.RelatedWorkItemId
    }
}

$parentWorkItem = $store.GetWorkItem($parentWorkItemId)
foreach($linkedItem in $parentWorkItem.Links)
{
    if ($linkedItem.LinkTypeEnd.Name -eq 'Child')
    {
        $childId = $linkedItem.RelatedWorkItemId
        $childItem = $store.GetWorkItem($childId)
        if ($childItem.State -ne 'Done')
        {
            Write-Output "Work item $($childItem.Id) has state $($childItem.State) which is not 'Done'."
            return
        }
    }
}

Write-Output "All work items related to the release have been marked as Done. The release can now be published. Setting state to INITIATION"
$parentWorkItem.State = 'Initiation'
$parentWorkItem.Save()
