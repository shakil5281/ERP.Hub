# Requires -RunAsAdministrator
<#
.SYNOPSIS
    Compiles Tailwind CSS, publishes the .NET 8 Blazor application, and hosts it on local IIS.
.DESCRIPTION
    This script automates the full deployment cycle for ERPHub:
    1. Validates prerequisites (Node, .NET 8 SDK, ASP.NET Core Hosting Bundle).
    2. Builds Tailwind CSS assets.
    3. Runs dotnet publish in Release mode to the target directory.
    4. Enables IIS features if not already configured.
    5. Configures IIS Application Pool ("No Managed Code") and Web Site.
    6. Configures ACL permission for IIS_IUSRS access.
#>

$ErrorActionPreference = "Stop"

# 1. Elevate to Administrator if not already running as Admin
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "=================================================================" -ForegroundColor Yellow
    Write-Host "This script must be run as Administrator." -ForegroundColor Yellow
    Write-Host "Relaunching in an elevated PowerShell session..." -ForegroundColor Yellow
    Write-Host "=================================================================" -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs -WorkingDirectory $PSScriptRoot
    Exit
}

# Ensure working directory is the script root directory
Set-Location $PSScriptRoot

# 2. Configuration Settings
$PublishDir = "C:\inetpub\wwwroot\ERPHub"
$SiteName = "ERPHub"
$Port = 8080
$AppPoolName = "ERPHubPool"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "          ERPHub Local IIS Publish & Setup Script               " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "Publish Directory : $PublishDir" -ForegroundColor White
Write-Host "IIS Site Name     : $SiteName" -ForegroundColor White
Write-Host "IIS Port          : $Port" -ForegroundColor White
Write-Host "IIS AppPool Name  : $AppPoolName" -ForegroundColor White
Write-Host "=================================================================" -ForegroundColor Cyan

# 3. Verify Prerequisites
Write-Host "[1/6] Verifying prerequisites..." -ForegroundColor Cyan

# Check dotnet SDK
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI was not found. Please install the .NET 8 SDK."
}
$dotnetVersion = (dotnet --version).Trim()
Write-Host "  -> Found .NET SDK: $dotnetVersion" -ForegroundColor Green

# Check Node / NPM for Tailwind
if (-not (Get-Command "npm" -ErrorAction SilentlyContinue)) {
    Write-Error "npm was not found. Please install Node.js (which includes npm) to compile Tailwind CSS."
}
$nodeVersion = (node --version).Trim()
Write-Host "  -> Found Node.js: $nodeVersion" -ForegroundColor Green

# Check ASP.NET Core Hosting Bundle for IIS
$aspNetCoreDll = "C:\Windows\System32\inetsrv\aspnetcore.dll"
$hasHostingBundle = Test-Path $aspNetCoreDll
if (-not $hasHostingBundle) {
    Write-Host "  [WARNING] ASP.NET Core Module for IIS (aspnetcore.dll) was not found in '$aspNetCoreDll'." -ForegroundColor Yellow
    Write-Host "  Please install the ASP.NET Core 8.0 Hosting Bundle: https://dotnet.microsoft.com/en-us/download/dotnet/8.0" -ForegroundColor Yellow
    Write-Host "  Continuing anyway, but IIS hosting will not work until this bundle is installed." -ForegroundColor Yellow
} else {
    Write-Host "  -> Found ASP.NET Core IIS Module (aspnetcore.dll)." -ForegroundColor Green
}

# 4. Build Tailwind CSS
Write-Host "`n[2/6] Building Tailwind CSS..." -ForegroundColor Cyan
if (-not (Test-Path "node_modules")) {
    Write-Host "  Installing npm packages..." -ForegroundColor White
    npm install
}
Write-Host "  Compiling CSS..." -ForegroundColor White
npm run build:css
Write-Host "  -> Tailwind CSS compilation complete." -ForegroundColor Green

# 5. dotnet publish
Write-Host "`n[3/6] Publishing application..." -ForegroundColor Cyan
if (Test-Path $PublishDir) {
    Write-Host "  Cleaning existing publish directory: $PublishDir..." -ForegroundColor White
    # Delete contents, keeping the folder itself
    Remove-Item "$PublishDir\*" -Recurse -Force -ErrorAction SilentlyContinue
} else {
    Write-Host "  Creating publish directory: $PublishDir..." -ForegroundColor White
    New-Item -ItemType Directory -Path $PublishDir -Force | Out-Null
}

# Run publish (skipping the duplicate Tailwind build target inside .csproj)
dotnet publish -c Release -o $PublishDir /p:SkipTailwind=true
Write-Host "  -> Publish completed successfully." -ForegroundColor Green

# 6. Enable IIS Windows Features
Write-Host "`n[4/6] Checking Windows Features (IIS)..." -ForegroundColor Cyan
if (Get-Command "Enable-WindowsOptionalFeature" -ErrorAction SilentlyContinue) {
    Write-Host "  Ensuring IIS and Web Server features are active..." -ForegroundColor White
    $features = @(
        "IIS-WebServerRole",
        "IIS-WebServer",
        "IIS-CommonHttpFeatures",
        "IIS-StaticContent",
        "IIS-DefaultDocument",
        "IIS-DirectoryBrowsing",
        "IIS-HttpErrors"
    )
    foreach ($feature in $features) {
        Write-Host "  Enabling feature: $feature..." -ForegroundColor White
        Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart -ErrorAction SilentlyContinue | Out-Null
    }
} else {
    Write-Host "  Get-Command Enable-WindowsOptionalFeature not available. Assuming IIS is already enabled." -ForegroundColor Yellow
}

# 7. Configure IIS Web Site and AppPool
Write-Host "`n[5/6] Configuring IIS Web Site & AppPool..." -ForegroundColor Cyan
Import-Module WebAdministration -ErrorAction SilentlyContinue

# Check if WebAdministration module is available
if (-not (Get-Module WebAdministration -ErrorAction SilentlyContinue)) {
    Write-Error "IIS WebAdministration module could not be loaded. Please ensure IIS Management Console is enabled in Windows Features."
}

# Create App Pool if it doesn't exist
if (-not (Test-Path "IIS:\AppPools\$AppPoolName")) {
    Write-Host "  Creating AppPool: $AppPoolName..." -ForegroundColor White
    New-WebAppPool $AppPoolName | Out-Null
} else {
    Write-Host "  AppPool '$AppPoolName' already exists." -ForegroundColor White
}

# Configure AppPool for .NET Core (No Managed Code)
Write-Host "  Configuring AppPool: No Managed Code, Integrated Mode..." -ForegroundColor White
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "managedPipelineMode" -Value 0 # Integrated

# Create/Update Site
if (Test-Path "IIS:\Sites\$SiteName") {
    Write-Host "  Website '$SiteName' already exists. Recreating it to map to $PublishDir..." -ForegroundColor White
    Remove-Website -Name $SiteName
}
Write-Host "  Creating Website '$SiteName' on port $Port..." -ForegroundColor White
New-Website -Name $SiteName -Port $Port -PhysicalPath $PublishDir -ApplicationPool $AppPoolName -Force | Out-Null
Write-Host "  -> IIS configuration completed successfully." -ForegroundColor Green

# 8. Set File System Permissions for IIS_IUSRS
Write-Host "`n[6/6] Applying folder security permissions..." -ForegroundColor Cyan
Write-Host "  Granting Read & Execute permissions to IIS_IUSRS..." -ForegroundColor White

$acl = Get-Acl $PublishDir
$permission = "IIS_IUSRS", "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
Set-Acl $PublishDir $acl

Write-Host "  -> File permissions successfully updated." -ForegroundColor Green

# 9. Finished
Write-Host "`n=================================================================" -ForegroundColor Green
Write-Host "          ERPHub successfully deployed to Local IIS!             " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
Write-Host " You can now access the portal at: http://localhost:$Port" -ForegroundColor Green
if (-not $hasHostingBundle) {
    Write-Host "`n [ALERT] Remember to install the ASP.NET Core Hosting Bundle 8.0!" -ForegroundColor Yellow
    Write-Host " Otherwise, the website will return an HTTP 500.19 Error." -ForegroundColor Yellow
}
Write-Host "=================================================================" -ForegroundColor Green
