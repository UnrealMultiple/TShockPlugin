#!/usr/bin/env pwsh
#Requires -Version 7

[CmdletBinding()]
param (
    [Parameter()]
    [switch] $All,
    [Parameter()]
    [switch] $NoPo,
    [Parameter()]
    [switch] $NoMo,
    [Parameter(Position = 0)]
    [string] $ProjectPath
)
$ProjectFile = $null
if ($ProjectPath) {
  $ProjectFile = Get-Item $ProjectPath
}

Set-Location $PSScriptRoot/..
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

function Format-To-Unix-Path-Style {
  param (
    [string] $file_path
  )
  
  $regex = [regex]::new('^#:.*', [System.Text.RegularExpressions.RegexOptions]::Multiline)

  $file_content = Get-Content -Path $file_path -Raw
  $formatted_content = $regex.Replace($file_content, { $args[0].ToString().Replace('\', '/') })
  if ([bool]::Parse($(git config --get core.autocrlf) ?? 'false')) {
    $formatted_content = $formatted_content -replace '((?<!\r)\n|\r(?!\n))', "`r`n"
  }
  else {
    $formatted_content = $formatted_content -replace '\r\n', "`n"
  }
  $formatted_content | Out-File -FilePath $file_path -NoNewline
}

function Update-I18n {
  param (
    [System.IO.FileSystemInfo] $project_file_path
  )
  $p = $project_file_path

  $pot = Join-Path $p.DirectoryName "i18n" "template.pot"
  New-Item -Path $p.DirectoryName -Name i18n -ItemType Directory -Force | Out-Null

  Write-Output "[$($p.Name)] generating template.pot..."
  dotnet tool run GetText.Extractor -u -o -s $p.FullName -t $pot

  if (!(Test-Path -Path $pot -PathType Leaf)) {
    Write-Output "[$($p.Name)] skipped!"
    continue
  }

  Format-To-Unix-Path-Style $pot

  $d = -split $(git diff --numstat $pot)
  if (($d.Length -ge 2) -and ([int]$d[0] -le 2 -and [int]$d[1] -le 2)) {
    git restore $pot
    Write-Output "[$($p.Name)] template.pot restored!"
  }

  if ($NoPo -and $NoMo) {
    continue
  }

  foreach ($t in @(Get-ChildItem $($p.DirectoryName)i18n/*.po)) {
    if (!$NoPo) {
      Write-Output "[$($p.Name)] [$($t.Name)] merging..."
      msgmerge --previous --update $t.FullName $pot
    }
    
    if (!$NoMo) {
      $mo = [System.IO.Path]::ChangeExtension($t.FullName, '.mo')
      Write-Output "[$($p.Name)] [$([System.IO.Path]::GetFileName($mo))] generating..."
      msgfmt -o $mo $t.FullName
    }
  }
}

if ($All) {
  foreach ($p in @(Get-ChildItem src/*/*.csproj)) {
    Update-I18n $p
  }
}
elseif ($ProjectPath) {
  Update-I18n $ProjectFile
}