#!/usr/bin/env pwsh
#Requires -Version 7

Set-Location $PSScriptRoot/..
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$ErrorActionPreference = 'Stop'

# preparing manifests
New-Item -Name ./bin/manifests -ItemType Directory -Force | Out-Null
foreach ($p in @(Get-ChildItem ./src/**/*.csproj)) {
  $manifestPath = "$($p.DirectoryName)/manifest.json"
  if (Test-Path $manifestPath -PathType Leaf) {
    Copy-Item $manifestPath "./bin/manifests/$($p.Basename).json"
  }
}
Copy-Item ./.config/submodule-manifests/* ./bin/manifests

$asm_to_dir = @{}
foreach ($p in @(Get-ChildItem ./src/**/*.csproj)) {
  $asm_to_dir[$p.BaseName] = Resolve-Path $p.Directory -Relative
}

function Get-PluginList {
  param (
    [string] $culture,
    [string] $text_yes,
    [string] $text_no
  )

  $dependencies = @{}
  Get-Content ./Plugins.json -Raw | ConvertFrom-Json | ForEach-Object {
    $dependencies[$_.AssemblyName] = $_.Dependencies
  }

  $infos = [ordered]@{}
  foreach ($jf in @(Get-ChildItem ./bin/manifests/*.json)) {
    $asm_name = $jf.BaseName
    $json = Get-Content $jf -Raw | ConvertFrom-Json -AsHashtable
    $infos[$asm_name] = [PSCustomObject]@{
      READMEUrl = $json["README.$culture"]?['READMEUrl'] ??
        $json['README']?['READMEUrl'] ??
        ((Test-Path "$($asm_to_dir[$asm_name])/README.$culture.md" -PathType Leaf) ?
          "$($asm_to_dir[$asm_name])/README.$culture.md" :
          "$($asm_to_dir[$asm_name])/README.md")
      Description = $json["README.$culture"]?['Description'] ??
        $json['README']?['Description']
      LanguageAvailable = $json["README.$culture"]?['LanguageAvailable'] ??
        (Test-Path "$($asm_to_dir[$asm_name])/i18n/$culture.po" -PathType Leaf)
      Dependencies = $dependencies[$asm_name] ?? @()
    }
  }

  return ($infos.GetEnumerator() | ForEach-Object {
    $name = $_.Key
    $rurl = $_.Value.READMEUrl
    $lang = $_.Value.LanguageAvailable ? $text_yes : $text_no
    $desc = $_.Value.Description
    $deps = ($_.Value.Dependencies | ForEach-Object { "[$_]($($infos[$_]?.READMEUrl))" }) -join ' '
    return $culture ?
      "| [$name]($rurl) | $lang | $desc | $deps |" :
      "| [$name]($rurl) | $desc | $deps |"
  }) -join "`n"
}

foreach ($tf in @(Get-ChildItem ./.config/readme-templates/*.md)) {
  $template = Get-Content $tf -Raw
  $regex = [regex]::new('<!--{.*?}-->')
  $readme = $regex.Replace($template, {
    $raw_cmd = $args[0].ToString()
    $cmds = $raw_cmd.Substring(5, $raw_cmd.Length-5-4) -split ','
    switch -CaseSensitive ($cmds[0]) {
      'PluginList' { Get-PluginList $cmds[1] $cmds[2] $cmds[3] }
      Default { "<!--# no interpolation string named $($cmds[0]) #-->" }
    }
  })
  $readme | Out-File -FilePath "./$($tf.BaseName).md" -NoNewline
}
