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

Write-Verbose "Start-Build: param projectCollection = $projectCollection"
Write-Verbose "Start-Build: param project = $project"
Write-Verbose "Start-Build: param id = $id"
Write-Verbose "Start-Build: param user = $user"
Write-Verbose "Start-Build: param tfsServerUri = $tfsServerUri"

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

$approvals = 0

$workItem = $store.GetWorkItem($id)
foreach($revision in $workItem.Revisions)
{
    $history = $revision.Fields['History'].Value
    if (($history -ne $null) -and ($history.StartsWith('Issue status: Approve:')))
    {
        $user = ($history.SubString('Issue status: Approve:'.Length)).Trim()
        Write-Output "Issue has been approved by $user"
        $approvals = $approvals + 1
    }

    if ($approvals -ge 2)
    {
        break
    }
}

if ($approvals -ge 2)
{
    $note = "Issue has been approved for completeness by multiple sources."
    Write-Output "$note. Setting state to DONE"
    $workItem.State = 'Done'
    $workItem.History = $note
    $workItem.Save()
}