#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory)]
    [string]$BuildType,

    [Parameter(Mandatory)]
    [string]$TargetFramework
)

$jsonContent = Get-Content -Path ".config/submodule_build.json" -Raw  | ConvertFrom-Json
New-Item -Path SubmoduleAssembly -ItemType Directory
foreach($submodule in $jsonContent.submodules)
{
    dotnet build $submodule.project_path -c $BuildType
    $assembly_path = $submodule.assembly_path -replace "{BuildType}", $BuildType -replace "{TargetFramework}" , $TargetFramework
    $pdb = $assembly_path -replace ".dll", ".pdb"
    Copy-Item -Path $assembly_path -Destination SubmoduleAssembly
    Copy-Item -Path $pdb -Destination SubmoduleAssembly
    Copy-Item -Path $submodule.readme -Destination (Join-Path -Path SubmoduleAssembly -ChildPath ($submodule.name + ".md"))
}