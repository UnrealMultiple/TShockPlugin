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
    Remove-Item ./out/$BuildType -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item ./SubmoduleAssembly -Recurse -Force -ErrorAction SilentlyContinue
    & $PSScriptRoot/submodule_build.ps1 -BuildType $BuildType -ErrorAction Stop
    dotnet build Plugin.sln -c $BuildType
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

New-Item -Name ./cache -ItemType Directory -Force
Remove-Item ./publish -Recurse -Force -ProgressAction SilentlyContinue -ErrorAction Ignore
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
            Invoke-GitHubRequest -Uri 'https://api.github.com/repos/WindFrost-CSFT/TShock/releases' | `
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
$proc = Start-Process -NoNewWindow -PassThru './TShock.Server' -ArgumentList '-dump-plugins-list-only','./manifests'
$proc | Wait-Process -Timeout 180 -ErrorAction SilentlyContinue -ErrorVariable timeouted
if ($timeouted) {
    $proc | Stop-Process
    throw "TShock.Server timeout!"
}
elseif ($proc.ExitCode -ne 0) {
    throw "TShock.Server error!"
}


if (-not $NoUpdateREADME) {
    & $PSScriptRoot/generate-readme.ps1
}

# Packing Plugins.zip
Set-Location $PSScriptRoot/..
Remove-Item ./out/Target -Recurse -Force -ProgressAction SilentlyContinue -ErrorAction Ignore
Remove-Item ./out/Plugins.zip -Recurse -Force -ProgressAction SilentlyContinue -ErrorAction Ignore
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
Copy-Item ./out/$BuildType/*.dll, ./out/$BuildType/*.pdb ./out/Target/Plugins/
Copy-Item ./SubmoduleAssembly/* ./out/Target/Plugins/
Copy-Item ./publish/Plugins.json, ./README*.md, ./Usage.txt, ./LICENSE ./out/Target/
# APM
New-Item -Path ./out/Target -Name Apm -ItemType Directory -Force
Copy-Item ./out/$BuildType/AutoPluginManager.* ./out/Target/Apm/

Compress-Archive -Path ./out/Target/* -DestinationPath ./out/Plugins.zip
