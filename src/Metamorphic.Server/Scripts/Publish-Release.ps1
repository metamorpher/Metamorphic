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

Write-Verbose "Publish-Release: param projectCollection = $projectCollection"
Write-Verbose "Publish-Release: param project = $project"
Write-Verbose "Publish-Release: param id = $id"
Write-Verbose "Publish-Release: param user = $user"
Write-Verbose "Publish-Release: param tfsServerUri = $tfsServerUri"

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

# Expecting something like: Products in release: [CardPromotions]
$buildAndBranchRegex = '^(?:Products\s*in\s*release:)\s*(?:\[)(.*?)(?:\])'

$description = $workItem.Item('Vista.Description')
if (-not ($description -match $versionNumberRegEx))
{
    throw "$description does not match the expected build comment of 'Products in release: [Build1, Build2, ...]'"
}

$products = ([regex]::Replace($description, $buildAndBranchRegex, '$1')).Split(',')

$note = "All products have been released."
Write-Output "$note. Setting state to DONE"
$workItem.State = 'Done'
$workItem.History = $note
$workItem.Save()

try
{
    $settingsText = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'metamorphic.settings.json'))
    $settings = ConvertFrom-Json $settingsText

    $to = "$($settings.Email)"
    $from = "The test pipeline <$($settings.Email)>"
    $subject = "Released products as per release '$($workItem.Title)'"
    $body = @"
    The following items have been released:
$($products | Foreach-Object { "    * " + "$_" + [System.Environment]::NewLine })
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