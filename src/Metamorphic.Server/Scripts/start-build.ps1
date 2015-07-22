[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string] $id,

    [ValidateNotNullOrEmpty()]
    [string] $user,

    [ValidateNotNullOrEmpty()]
    [string] $comment
)

# Expecting something like: Development complete. Test builds: [Cinema, FilmProgramming, HeadOffice] on branches [Main, branches/4.5.2]

# parse the comment string


$builds = @("a", "b")
$branches = @("c", "d" )

# Start bulids