# Steam Runtime Setup Script
# This script prepares the environment for building with Steam Runtime

$ErrorActionPreference = "Stop"
$ProjectRoot = "H:\KrissJourney"
$BuildRoot = Join-Path -Path $ProjectRoot -ChildPath "SteamRuntimeBuild"
$OutputDir = Join-Path -Path $ProjectRoot -ChildPath "SteamRuntimeBuild\output"

# Create output directory if it doesn't exist
if (-not (Test-Path $OutputDir)) {
    New-Item -Path $OutputDir -ItemType Directory -Force | Out-Null
}

Write-Host "Steam Runtime build environment set up!" -ForegroundColor Green
Write-Host "Build files located in: $BuildRoot" -ForegroundColor Cyan
Write-Host "Output will be placed in: $OutputDir" -ForegroundColor Cyan

Write-Host "`nTo build for Steam Runtime, follow these steps:" -ForegroundColor Yellow
Write-Host "1. Install Docker Desktop for Windows" -ForegroundColor Yellow
Write-Host "2. Run the build script: .\SteamRuntimeBuild\build-docker.ps1" -ForegroundColor Yellow