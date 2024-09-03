#!/usr/bin/env pwsh

$PSDefaultParameterValues['*:Encoding'] = 'utf8'

if ($PSVersionTable.PSVersion.Major -lt 7) {
  Write-Output 'no more powershell 5 plz'
  exit
}

function format_to_unix_path_style {
  param (
    [string] $file_path
  )
  
  $regex = [regex]::new('^#:.*', [System.Text.RegularExpressions.RegexOptions]::Multiline)

  $file_content = Get-Content -Path $file_path -Raw
  $formatted_content = $regex.Replace($file_content, { $args[0].ToString().Replace('\', '/') })
  if ([bool]::Parse($(git config --get core.autocrlf))) {
    $formatted_content = $formatted_content -replace '((?<!\r)\n|\r(?!\n))', "`r`n"
  }
  else {
    $formatted_content = $formatted_content -replace '\r\n', "`n"
  }
  $formatted_content | Out-File -FilePath $file_path -NoNewline
}

foreach ($p in @(Get-ChildItem $PSScriptRoot/../src/*/*.csproj)) {
  $pot = Join-Path $p.DirectoryName "i18n" "template.pot"
  New-Item -Path $p.DirectoryName -Name i18n -ItemType Directory -Force | Out-Null

  Write-Output "[$($p.Name)] generating template.pot..."
  dotnet tool run GetText.Extractor -u -o -s $p.FullName -t $pot

  if (!(Test-Path -Path $pot -PathType Leaf)) {
    Write-Output "[$($p.Name)] skipped!"
    continue
  }

  format_to_unix_path_style($pot)

  $d = $(git diff --numstat $pot) -split "\s+"
  if (($d.Length -ge 2) -and ($d[0] -le 2 -and $d[1] -le 2)) {
    Write-Output "[$($p.Name)] template.pot restored!"
    git checkout $pot
    continue
  }

  foreach ($t in @(Get-ChildItem $($p.DirectoryName)i18n/*.po)) {
    Write-Output "[$($p.Name)] [$($t.Name)] merging..."
    msgmerge --previous --update $t.FullName $pot
    format_to_unix_path_style($t.FullName)
  }
}

