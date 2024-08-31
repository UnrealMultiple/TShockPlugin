#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $false)]
    [string]$Model
)

foreach ($p in @(ls $PSScriptRoot/../src/*/*.csproj))  {
    if (-not (Test-Path -Path "$($p.DirectoryName)/i18n" -PathType Container)) {
        if($Model -eq "auto"){
            New-Item -ItemType Directory -Path "$($p.DirectoryName)/i18n"
    }
    
    $pot = "$([System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot"))"
    GetText.Extractor -s $p.FullName -t $pot
    
    foreach ($t in @(ls "$($p.DirectoryName)/i18n/*.po"))  {
        msgmerge --previous --update $t $pot
    }
}