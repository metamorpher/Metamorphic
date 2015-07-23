[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string] $build,
    
    [ValidateNotNullOrEmpty()]
    [string] $buildNumber,

    [ValidateNotNullOrEmpty()]
    [string] $user,

    [ValidateNotNullOrEmpty()]
    [string] $reason
)

Write-Verbose "Notify-Tester: param build = $build"
Write-Verbose "Notify-Tester: param buildNumber = $buildNumber"
Write-Verbose "Notify-Tester: param user = $user"
Write-Verbose "Notify-Tester: param reason = $reason"

$ErrorActionPreference = 'Stop'

$commonParameterSwitches =
    @{
        Verbose = $PSBoundParameters.ContainsKey('Verbose');
        Debug = $PSBoundParameters.ContainsKey('Debug');
        ErrorAction = "Stop"
    }

# If something goes wrong then (for now) we want to stop the world
try
{
    $settingsText = [System.IO.File]::ReadAllText((Join-Path $PSScriptRoot 'metamorphic.settings.json'))
    $settings = ConvertFrom-Json $settingsText

    $to = "$($settings.Email)"
    $from = "The test pipeline <$($settings.Email)>"
    $subject = "Build ready for testing"
    $body = @"
    A build of $build has completed. You can find the output in the standard location.
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