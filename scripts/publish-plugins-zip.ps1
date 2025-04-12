#!/usr/bin/env pwsh

[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $GithubToken = $null,

    [Parameter()]
    [switch]
    $NoBuild,

    [Parameter()]
    [string]
    $BuildType = 'Debug',

    [Parameter()]
    [switch]
    $NoCache,

    [Parameter()]
    [switch]
    $NoUpdateREADME
)

Set-Location $PSScriptRoot/..

# Build plugins
if (-not $NoBuild) {
    & $PSScriptRoot/submodule_build.ps1 -BuildType $BuildType
    dotnet build Plugin.sln -c $BuildType
}

New-Item -Name ./cache -ItemType Directory -Force
if (Test-Path ./publish) {
    Remove-Item ./publish -Recurse -ProgressAction SilentlyContinue
}
New-Item -Name ./publish -ItemType Directory -Force

function Get-TShockZip {
    param (
        [Parameter(Position = 0)]
        [string] $OutFile
    )
    function Invoke-GitHubRequest {
        param(
            [Parameter(Mandatory = $true)][string]$Uri,
            [Object]$Body,
            [String]$ContentType,
            [PSCredential]$Credential,
            [System.Collections.IDictionary]$Headers,
            [String]$InFile,
            [Int32]$MaximumRedirection,
            [Microsoft.PowerShell.Commands.WebRequestMethod]$Method,
            [String]$OutFile,
            [String]$SessionVariable,
            [Int32]$TimeoutSec,
            [String]$TransferEncoding,
            [String]$UserAgent,
            [Microsoft.PowerShell.Commands.WebRequestSession]$Session
        )
        $p = @{}
        $PSBoundParameters.GetEnumerator() | ForEach-Object { $p.Add( $_.Key, $_.Value) }
        if ($GithubToken) {
            $secureGithubToken = ConvertTo-SecureString $GithubToken -AsPlainText -Force
            return Invoke-WebRequest -Authentication Bearer -Token $secureGithubToken @p
        }
        else {
            return Invoke-WebRequest @p
        }
    }

    $rid = if ([System.Environment]::OSVersion.Platform -Match "Unix") { "linux-(x64|amd64)" } else { "win-(x64|amd64)" }
    Invoke-GitHubRequest -Uri ( `
            Invoke-GitHubRequest -Uri 'https://api.github.com/repos/Pryaxis/TShock/releases' | `
            ConvertFrom-Json | `
            Select-Object -First 1 -ExpandProperty assets | `
            Where-Object browser_download_url -Match $rid | `
            Select-Object -ExpandProperty browser_download_url) `
        -OutFile $OutFile
}

# Prepare TShock
if (-not(Test-Path ./cache/TShock.zip -PathType Leaf) -or $NoCache) {
    Get-TShockZip ./cache/TShock.zip
}
Expand-Archive ./cache/TShock.zip -DestinationPath ./publish
if ([System.Environment]::OSVersion.Platform -Match "Unix") {
    tar xvf ./publish/TShock-Beta-linux-x64-Release.tar --directory ./publish
}

# Prepare plugin dlls
Copy-Item ./out/**/*.dll ./publish/ServerPlugins/
Copy-Item ./SubmoduleAssembly/*.dll ./publish/ServerPlugins/

# Prepare manifests
Set-Location $PSScriptRoot/../publish
New-Item -Name ./manifests -ItemType Directory -Force
foreach ($p in @(Get-ChildItem ../src/**/*.csproj)) {
    $manifestPath = Join-Path $p.DirectoryName manifest.json
    if (Test-Path $manifestPath -PathType Leaf) {
        Copy-Item $manifestPath $(Join-Path manifests "$($p.Basename).json")
    }
}
Copy-Item ../.config/submodule-manifests/* ./manifests

# Start generating plugin list
$job = Start-ThreadJob -ScriptBlock {
    ./TShock.Server -dump-plugins-list-only ./manifests
}
$job | Wait-Job -Timeout 180 | Out-Null
$job | Receive-Job
if ($job | Where-Object {$_.State -ne "Completed"}) {
    throw "TShock.Server timeout!"
}

if (-not $NoUpdateREADME) {
    & $PSScriptRoot/generate-readme.ps1
}

# Packing Plugins.zip
Set-Location $PSScriptRoot/..
if (Test-Path ./out/Target) {
    Remove-Item ./out/Target -Recurse -ProgressAction SilentlyContinue
}
if (Test-Path ./out/Plugins.zip) {
    Remove-Item ./out/Plugins.zip -Recurse -ProgressAction SilentlyContinue
}
New-Item -Path ./out/Target -Name Plugins -ItemType Directory -Force
$ErrorActionPreference = "SilentlyContinue"
foreach ($p in @(Get-ChildItem src/**/*.csproj)) {             
    foreach ($r in @(Get-ChildItem "$($p.Directory)/README*.md")) {
        $ext_parts = ($r.Name -split '\.')
        $ext = $ext_parts[1..$ext_parts.Length] -join '.'
        Copy-Item $r "out/Target/Plugins/$($p.BaseName).$ext"
    }
}
$ErrorActionPreference = "Continue"
Copy-Item ./out/**/*.dll, ./out/**/*.pdb ./out/Target/Plugins/
Copy-Item ./SubmoduleAssembly/* ./out/Target/Plugins/
Copy-Item ./publish/Plugins.json, ./README*.md, ./Usage.txt, ./LICENSE ./out/Target/

Compress-Archive -Path ./out/Target/* -DestinationPath ./out/Plugins.zip
