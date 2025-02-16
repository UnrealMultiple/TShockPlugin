#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory)]
    [string]$BuildType,

    [Parameter(Mandatory)]
    [string]$TargetFramework
)

function Join-Repo-Root {
    param (
        [Parameter(Mandatory, Position=0, ValueFromRemainingArguments)]
        [string[]] $Paths
    )
    return Join-Path $PSScriptRoot '..' @Paths
}

$jsonContent = Get-Content -Path $(Join-Repo-Root '.config/submodule_build.json') -Raw  | ConvertFrom-Json
New-Item -Path $(Join-Repo-Root 'SubmoduleAssembly') -ItemType Directory -Force
foreach($submodule in $jsonContent.submodules)
{
    dotnet build $(Join-Repo-Root $submodule.project_path) -c $BuildType
    $assembly_path = $(Join-Repo-Root $submodule.assembly_path) -replace '{BuildType}', $BuildType -replace '{TargetFramework}', $TargetFramework
    $pdb = $assembly_path -replace '.dll', '.pdb'
    Copy-Item -Path $assembly_path -Destination $(Join-Repo-Root 'SubmoduleAssembly')
    Copy-Item -Path $pdb -Destination $(Join-Repo-Root 'SubmoduleAssembly')
    if (-not [string]::IsNullOrEmpty($submodule.readme)) {
            Copy-Item -Path $(Join-Repo-Root $submodule.readme) -Destination $(Join-Repo-Root 'SubmoduleAssembly' ($submodule.name + ".md"))
    }
}