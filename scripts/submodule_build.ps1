param(
    [Parameter(Mandatory=$true)]
    [string]$BuildType,

    [Parameter(Mandatory=$true)]
    [string]$TargetFramework
)

$jsonContent = Get-Content -Path ".config/submodule_build.json" -Raw  | ConvertFrom-Json
New-Item -Path SubmoduleAssembly -ItemType Directory
foreach($submodule in $jsonContent.submodules)
{
    $cmd = "dotnet build {0} -c {1}" -f $submodule.project_path, $BuildType
    Invoke-Expression $cmd
    $assembly_path = $submodule.assembly_path -replace "{BuildType}", $BuildType -replace "{TargetFramework}" , $TargetFramework
    $pdb = $assembly_path -replace ".dll", ".pdb"
    Copy-Item -Path $assembly_path -Destination SubmoduleAssembly
    Copy-Item -Path $pdb -Destination SubmoduleAssembly
    Copy-Item -Path $submodule.readme -Destination (Join-Path -Path SubmoduleAssembly -ChildPath ($submodule.name + ".md"))
}