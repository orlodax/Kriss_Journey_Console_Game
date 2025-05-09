# Post-build processing for Steam Runtime build
# Generates Steam-specific files and prepares for upload

$ErrorActionPreference = "Stop"
$ProjectRoot = "H:\KrissJourney"
$BuildRoot = Join-Path -Path $ProjectRoot -ChildPath "SteamRuntimeBuild"
$OutputDir = Join-Path -Path $BuildRoot -ChildPath "output"

Write-Host "Running post-build processing for Steam Runtime..." -ForegroundColor Cyan

# Make sure the output directory exists and contains built files
if (-not (Test-Path (Join-Path -Path $OutputDir -ChildPath "Kriss"))) {
    Write-Host "Build output not found. Please run the build process first." -ForegroundColor Red
    exit 1
}

# Ensure the Steam launcher script exists in the output directory
$steamLauncherSource = Join-Path -Path $ProjectRoot -ChildPath "steam-launcher.sh"
$steamLauncherTarget = Join-Path -Path $OutputDir -ChildPath "steam-launcher.sh"

if (-not (Test-Path $steamLauncherTarget)) {
    if (Test-Path $steamLauncherSource) {
        Write-Host "Copying Steam launcher script to output directory..." -ForegroundColor Cyan
        Copy-Item -Path $steamLauncherSource -Destination $steamLauncherTarget -Force
    }
    else {
        Write-Host "ERROR: Steam launcher script not found." -ForegroundColor Red
    }

    # Ensure the launcher script has the correct line endings (LF instead of CRLF)
    $content = Get-Content -Path $steamLauncherTarget -Raw
    $content = $content -replace "`r`n", "`n"
    [System.IO.File]::WriteAllText($steamLauncherTarget, $content)
}

# Verify that the essential files are present
$essentialFiles = @(
    "Kriss",
    "steam-launcher.sh"
)

$missingFiles = $essentialFiles | Where-Object { -not (Test-Path (Join-Path -Path $OutputDir -ChildPath $_)) }
if ($missingFiles.Count -gt 0) {
    Write-Host "Warning: The following essential files are missing:" -ForegroundColor Yellow
    $missingFiles | ForEach-Object { Write-Host "- $_" -ForegroundColor Yellow }
}
else {
    Write-Host "All essential files are present in the output directory." -ForegroundColor Green
}

Write-Host "Post-build processing complete!" -ForegroundColor Green
Write-Host "Your Steam Linux build is ready in: $OutputDir" -ForegroundColor Cyan