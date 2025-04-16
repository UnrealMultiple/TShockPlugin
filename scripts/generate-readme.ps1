#!/usr/bin/env pwsh
#Requires -Version 7

Set-Location $PSScriptRoot/..
$PSDefaultParameterValues['*:Encoding'] = 'utf8'
$ErrorActionPreference = 'Stop'

# preparing manifests
Remove-Item ./publish/manifests -Recurse -Force -ProgressAction SilentlyContinue -ErrorAction Ignore
New-Item -Name ./publish/manifests -ItemType Directory -Force | Out-Null
foreach ($p in @(Get-ChildItem ./src/**/*.csproj)) {
  $manifestPath = "$($p.DirectoryName)/manifest.json"
  if (Test-Path $manifestPath -PathType Leaf) {
    Copy-Item $manifestPath "./publish/manifests/$($p.Basename).json"
  }
}
Copy-Item ./.config/submodule-manifests/* ./publish/manifests

$asm_to_dir = @{}
foreach ($p in @(Get-ChildItem ./src/**/*.csproj)) {
  $asm_to_dir[$p.BaseName] = Resolve-Path $p.Directory -Relative
}

function Get-TranslationPercentage {
  param (
    [string] $po_path
  )

  if (-not (Test-Path $po_path -PathType Leaf)) {
    return '0.0%'
  }

  $regex_untranslated = [regex]::new('(?m)^msgstr ""(?!\n")')
  $regex_total_plus_1 = [regex]::new('(?m)^msgstr "')
  $po_content = Get-Content $po_path -Raw

  $untranslated_count = $regex_untranslated.Matches($po_content).Count
  $total_count = $regex_total_plus_1.Matches($po_content).Count - 1
  if ($total_count -eq 0) {
    return '0.0%'
  }
  return '{0:0.0}%' -f ([float]($total_count - $untranslated_count) / $total_count * 100)
}

function Get-PluginList {
  param (
    [string] $culture
  )

  $dependencies = @{}
  Get-Content ./Plugins.json -Raw | ConvertFrom-Json | ForEach-Object {
    $dependencies[$_.AssemblyName] = $_.Dependencies
  }

  $infos = [ordered]@{}
  foreach ($jf in @(Get-ChildItem ./publish/manifests/*.json)) {
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
      TranslationPercentage = $json["README.$culture"]?['LanguageAvailable'] ? 
        '100.0%' :
        (Get-TranslationPercentage "$($asm_to_dir[$asm_name])/i18n/$culture.po")
      Dependencies = $dependencies[$asm_name] ?? @()
    }
  }

  return ($infos.GetEnumerator() | ForEach-Object {
    $name = $_.Key
    $rurl = $_.Value.READMEUrl
    $lang = $_.Value.TranslationPercentage
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
      'PluginList' { Get-PluginList $cmds[1] }
      Default { "<!--# no interpolation string named $($cmds[0]) #-->" }
    }
  })
  $readme | Out-File -FilePath "./$($tf.BaseName).md" -NoNewline
}
