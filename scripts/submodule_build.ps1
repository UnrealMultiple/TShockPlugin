param(
    [Parameter(Mandatory=$true)]
    [string]$BuildType,

    [Parameter(Mandatory=$true)]
    [string]$TargetFramework
)

$jsonContent = Get-Content -Path ".config/submodule_build.json" -Raw  | ConvertFrom-Json

foreach($submodule in $jsonContent.submodules)
{
    $command = "dotnet build {0} -c {1}" -f $submodule.project_path, $BuildType
    $assembly_path = $submodule.assembly_path -replace "{BuildType}", $BuildType -replace "{TargetFramework}" , $TargetFramework
    $pdb = $assembly_path -replace ".dll", ".pdb"
    Copy-Item -Path $assembly_path -Destination out/Debug/
    Copy-Item -Path $pdb -Destination out/Debug/
    Invoke-Expression $command
}