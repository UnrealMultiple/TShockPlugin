 param(
    [Parameter(Mandatory)]
    [string]$t
)

$ext = [System.IO.Path]::GetExtension($t)
if($t -eq "*"){
    foreach ($p in @(Get-ChildItem src/**/*.csproj))  {
        $pot = [System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot")
        New-Item -Path $p.DirectoryName -Name i18n -ItemType Directory -Force
        dotnet tool run GetText.Extractor -u -o -s $p.FullName -t $pot
    }
}elseif((Test-Path $t) -and $ext -eq ".csproj"){
    $FullName = [System.IO.Path]::GetFullPath($t)
    $dirName = [System.IO.Path]::GetDirectoryName($t)
    $pot = [System.IO.Path]::Combine($dirName, "i18n", "template.pot")
    New-Item -Path $dirName -Name i18n -ItemType Directory -Force
    dotnet tool run GetText.Extractor -u -o -s $FullName -t $pot
}else{
    Write-Host "csproj file Path Not Exist!"
}

 
