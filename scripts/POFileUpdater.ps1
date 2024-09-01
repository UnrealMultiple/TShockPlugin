#!/usr/bin/env pwsh

foreach ($p in @(ls $PSScriptRoot/../src/*/*.csproj))  {
    if (-not (Test-Path -Path "$($p.DirectoryName)/i18n" -PathType Container)) {
        continue
    }

    echo "[$p] generating template.pot..."
    $pot = "$([System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot"))"
    dotnet tool run GetText.Extractor -u -o -s $p.FullName -t $pot
    
    $pot_diff = $(git diff --numstat $pot) -split '\t'
    if ($pot_diff[0] -eq '2' -and $pot_diff[1] -eq '2') {
        echo "[$p] template.pot no diff except date changes, restoring..."
        git restore $pot
#        continue
    }
    
    foreach ($t in @(ls "$($p.DirectoryName)/i18n/*.po"))  {
        echo "[$t] merging..."
        msgmerge --previous --update $t $pot
    }
}