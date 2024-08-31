#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $false)]
    [string]$Model
)

foreach ($p in @(ls $PSScriptRoot/../src/*/*.csproj))  {
    $I18nPath = "$($p.DirectoryName)/i18n"
    if (-not (Test-Path -Path $I18nPath -PathType Container)) {
        if($Model -eq "auto"){
            New-Item -ItemType Directory -Path $I18nPath
        }else{
            continue
        }
    }
    
    $pot = "$([System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot"))"
    GetText.Extractor -s $p.FullName -t $pot
    
    foreach ($t in @(ls "$($p.DirectoryName)/i18n/*.po"))  {
        msgmerge --previous --update $t $pot
    }
}