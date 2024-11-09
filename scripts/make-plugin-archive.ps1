#!/usr/bin/env pwsh
#Requires -Version 7

Set-Location $PSScriptRoot/..
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

# ./scripts/submodule_build.ps1 -BuildType Debug -TargetFramework net6.0
# dotnet build Plugin.sln -c Debug

Remove-Item -Recurse -Force bin
New-Item -Name bin -ItemType Directory
Expand-Archive out/TShock.zip -DestinationPath bin
if ([System.Environment]::OSVersion.Platform -Match "Unix") {
    tar xvf bin/TShock-Beta-linux-x64-Release.tar --directory bin
}
Copy-Item out/**/*.dll bin/ServerPlugins/
Copy-Item SubmoduleAssembly/*.dll bin/ServerPlugins/
Set-Location bin
./TShock.Server -dump-plugins-list-only
Set-Location ../
Remove-Item -Recurse -Force out/Target
New-Item -Path out/Target -Name Plugins -ItemType Directory
$ErrorActionPreference = "SilentlyContinue"
foreach ($p in @(Get-ChildItem src/**/*.csproj))  {
Copy-Item "$($p.DirectoryName)/README.md" "out/Target/Plugins/$($p.Directory.Name).md"
Copy-Item "$($p.DirectoryName)/README_EN.md" "out/Target/Plugins/$($p.Directory.Name)_EN.md"
}
$ErrorActionPreference = "Continue"
Copy-Item out/**/*.dll,out/**/*.pdb out/Target/Plugins/
Copy-Item SubmoduleAssembly/* out/Target/Plugins/
Copy-Item bin/Plugins.json,README.md,Usage.txt,LICENSE out/Target/
Set-Location ./out/Target
7z a -tzip ../../bin/Plugins.zip ./*
Set-Location ../../
Copy-Item ./bin/Plugins.json,./bin/Plugins.zip ../tmp
