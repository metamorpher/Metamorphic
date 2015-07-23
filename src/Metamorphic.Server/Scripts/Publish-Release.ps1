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

$workItem = $store.GetWorkItem($id)

try
{
    $settingsText = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'metamorphic.settings.json'))
    $settings = ConvertFrom-Json $settingsText

    $to = "$($settings.Email)"
    $from = "The test pipeline <$($settings.Email)>"
    $subject = "Release '$($workItem.Title)' is ready to be published"
    $body = @"
    All the linked children of release item $($workItem.Id) have been marked as done. You can now
    publish this release.
"@

    $msg = New-Object System.Net.Mail.MailMessage $from, $to, $subject, $body
    $msg.IsBodyHTML = $true

    $SmtpClient = New-Object system.net.mail.smtpClient
    $SmtpClient.host = $settings.SmtpServer

    Write-Output "Sending email: $msg"
    $SmtpClient.Send($msg)

    Write-Output "Email to $to send successfully"
}
catch
{
    Write-Error "Unable to send email with subject $subject to $to. Error was: "
}