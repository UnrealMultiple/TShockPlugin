Add-Type -AssemblyName System.Windows.Forms

function Get-DownloadUrl {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$false, Position=0)]
        [string]$url = "https://api.github.com/repos/Pryaxis/TShock/releases/latest"
    )

    process {
        try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing
            
            # 检查HTTP状态码
            if ($response.StatusCode -ne 200) {
                throw "Failed to retrieve the resource at '$url'. Status code: $($response.StatusCode)"
            }

            $json = $response.Content | ConvertFrom-Json
      
            if (-not ($json -and $json.assets[3].browser_download_url)) {
                throw "The JSON content at '$url' does not contain a valid 'browser_download_url' property."
            }

            return $json.assets[3].browser_download_url
        }
        catch [System.Net.WebException], [System.Management.Automation.PSInvocationException] {
            # 处理可能的网络异常和JSON解析异常
            throw "Error when attempting to get version from '$url': $_"
        }
    }
}

function Get-Dotnet{
    $dotnetVersions = dotnet --list-runtimes
    $hasNet6OrHigher = $false
    foreach ($version in $dotnetVersions) {
        if ($version -match "^Microsoft\.NETCore\.App\s+(\d+\.\d+)\.") {
            $majorVersion = [int]$Matches[1]
            if ($majorVersion -ge 6) {
                $hasNet6OrHigher = $true
                break
            }
        }
    }
    if(-not $hasNet6OrHigher){
        Write-Host ".net6 is not installed, start installing .net"
        $scriptUrl = "https://dot.net/v1/dotnet-install.ps1"
        # 下载并执行安装脚本
        Invoke-WebRequest -Uri $scriptUrl -OutFile "dotnet-install.ps1"
        powershell -ExecutionPolicy ByPass -File ./dotnet-install.ps1 -Runtime "dotnet" -NoPath
    }
}


function Get-TShockZip{
     param(
        [Parameter(Mandatory=$true)]
        [string]$proxy
    )

     process {
        try {
            $download_url = Get-DownloadUrl
            $url = $proxy + $download_url
            if($null -eq $download_url){
                throw "Not Get TShock Version Info!"
            }
            Write-Host "Download $url...."
            #禁用进度条，进度条可能会导致下载变慢!
            #$ProgressPreference = 'SilentlyContinue'
            $zipFile = "./TShock.zip"

            $response = Invoke-WebRequest -Uri $url -UseBasicParsing    
            # 检查HTTP状态码
            if ($response.StatusCode -ne 200) {
                throw "Failed to retrieve the resource at '$url'. Status code: $($response.StatusCode)"
            }
            [IO.File]::WriteAllBytes($zipFile, $response.Content)
            Write-Host "Ready to start unzipping TShock.zip...."
            Expand-Archive -Path "./TShock.zip" -DestinationPath "./TShockServer/"
            Remove-Item -Path $zipFile -Force
            $result = [System.Windows.Forms.MessageBox]::Show("Start TShock.Server ?", "Hint", [System.Windows.Forms.MessageBoxButtons]::YesNo, [System.Windows.Forms.MessageBoxIcon]::Question)
            if ($result -eq [System.Windows.Forms.DialogResult]::Yes) {
                Set-Location ./TShockServer
                Start-Process -FilePath ./TShock.Server.exe -ArgumentList "-lang 7"
            }
        }catch [System.Net.WebException], [System.Management.Automation.PSInvocationException] {
            # 处理可能的网络异常
            throw "Error when attempting to get zip from '$url': $_"
        }
    }
}

Get-Dotnet
$proxys = "https://gh.ddlc.top/","https://gh.llkk.cc/","https://mirror.ghproxy.com/","https://github.moeyy.xyz/","https://ghproxy.net/"
Write-Host "Places Choice Proxy Download TShock!"
$i = 1
foreach($url in $proxys){
    Write-Host $i"." $url
    $i = $i + 1
}
$choice = Read-Host
$index = [int]$choice
if ($proxys -ge 0 -and ($index -1) -lt $proxys.Length){
    Get-TShockZip -proxy $proxys[$index - 1]
}else{
    Write-Host "Input Number Error!"
}