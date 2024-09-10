

$jsonContent = Get-Content -Path ".config/submodule_build.json" -Raw  | ConvertFrom-Json

foreach($submodule in $jsonContent.submodules)
{
    $command = "dotnet build $submodule.project_path"
    & $command
}