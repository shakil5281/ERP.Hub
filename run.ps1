<#
.SYNOPSIS
    Restores NuGet packages, builds, and runs the ERPHub Blazor application.
.DESCRIPTION
    Automates the local development workflow:
    1. Validates the .NET SDK.
    2. Runs dotnet restore.
    3. Runs dotnet build.
    4. Runs dotnet run (unless -NoRun is specified).
.PARAMETER Configuration
    Build configuration. Defaults to Debug for local development.
.PARAMETER LaunchProfile
    Launch profile from Properties/launchSettings.json. Defaults to http.
.PARAMETER NoRun
    Restore and build only; do not start the application.
.EXAMPLE
    .\restore-build-run.ps1
.EXAMPLE
    .\restore-build-run.ps1 -Configuration Release -LaunchProfile https
.EXAMPLE
    .\restore-build-run.ps1 -NoRun
#>

param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug",

    [ValidateSet("http", "https")]
    [string] $LaunchProfile = "http",

    [switch] $NoRun
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "          ERPHub Restore, Build & Run                           " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "Project Root      : $PSScriptRoot" -ForegroundColor White
Write-Host "Configuration     : $Configuration" -ForegroundColor White
Write-Host "Launch Profile    : $LaunchProfile" -ForegroundColor White
Write-Host "Run Application   : $(-not $NoRun)" -ForegroundColor White
Write-Host "=================================================================" -ForegroundColor Cyan

Write-Host "[1/4] Verifying prerequisites..." -ForegroundColor Cyan
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI was not found. Install the .NET SDK: https://dotnet.microsoft.com/download"
}
$dotnetVersion = (dotnet --version).Trim()
Write-Host "  -> Found .NET SDK: $dotnetVersion" -ForegroundColor Green

Write-Host "`n[2/4] Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore ERPHub.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "  -> Restore completed successfully." -ForegroundColor Green

Write-Host "`n[3/4] Building project..." -ForegroundColor Cyan
dotnet build ERPHub.csproj -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host "  -> Build completed successfully." -ForegroundColor Green

if ($NoRun) {
    Write-Host "`n=================================================================" -ForegroundColor Green
    Write-Host "          Restore and build completed (run skipped).            " -ForegroundColor Green
    Write-Host "=================================================================" -ForegroundColor Green
    exit 0
}

Write-Host "`n[4/4] Starting application..." -ForegroundColor Cyan
Write-Host "  Press Ctrl+C to stop the server." -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan

dotnet run --project ERPHub.csproj -c $Configuration --no-build --launch-profile $LaunchProfile
exit $LASTEXITCODE
