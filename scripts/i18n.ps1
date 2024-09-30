 foreach ($p in @(Get-ChildItem src/**/*.csproj))  {
    $pot = [System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot")
    New-Item -Path $p.DirectoryName -Name i18n -ItemType Directory -Force
    dotnet tool run GetText.Extractor -u -o -s $p.FullName -t $pot
}