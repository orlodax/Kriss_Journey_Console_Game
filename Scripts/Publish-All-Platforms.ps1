$runtimes = @(
    @{ Name = "Windows x64"; Rid = "win-x64" },
    @{ Name = "macOS x64"; Rid = "osx-x64" }
)

# Call Steam Runtime build script for Linux with clean flag
Write-Host "Starting Steam Runtime build for Linux..." -ForegroundColor Cyan
$steamBuildScript = "H:\KrissJourney\SteamRuntimeBuild\build-docker.ps1"
if (Test-Path $steamBuildScript) {
    & $steamBuildScript -Clean
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Steam Runtime build completed successfully" -ForegroundColor Green
    }
    else {
        Write-Host "Steam Runtime build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    }
}
else {
    Write-Host "Steam Runtime build script not found at: $steamBuildScript" -ForegroundColor Red
}

# Build Windows and macOS platforms in parallel
$runtimes | ForEach-Object -Parallel {
    $runtime = $_.rid
    Write-Host "Publishing for $runtime..." -ForegroundColor Cyan
    $process = Start-Process -FilePath "dotnet" -ArgumentList "publish -c Release -r $runtime -p:PublishSingleFile=true --self-contained true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName=Kriss" -WorkingDirectory "H:\KrissJourney\Kriss" -NoNewWindow -PassThru -RedirectStandardOutput "$runtime-output.log"
    $process.WaitForExit()
    
    $output = Get-Content -Path "$runtime-output.log" -Raw
    Write-Host $output
    Write-Host "Publish completed with exit code: $($process.ExitCode)" -ForegroundColor Green
}

Write-Host "Publishing completed for Windows and macOS platforms." -ForegroundColor Yellow

# Copy and modify the launcher script for macOS
$runtime = "osx-x64"
$publishDir = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish"
$linuxLauncherScript = "H:\KrissJourney\krissLauncher.sh"
$macosLauncherScript = Join-Path -Path $publishDir -ChildPath "krissLauncher.sh"

Write-Host "Creating macOS launcher script..." -ForegroundColor Cyan

# Read the Linux launcher script
$launcherContent = Get-Content -Path $linuxLauncherScript -Raw

# Modify it with macOS-specific adjustments
$macosLauncherContent = $launcherContent -replace "# Version: 2.3 - With improved terminal detection", "# Version: 2.3 - macOS Steam Edition"

# Add macOS-specific variable
$macosLauncherContent = $macosLauncherContent -replace "OS_TYPE=""unknown""", "OS_TYPE=""macos"""

# Replace Unix line endings to ensure proper execution on macOS
$macosLauncherContent = $macosLauncherContent -replace "`r`n", "`n"

# Write the modified launcher script to the macOS publish directory
[System.IO.File]::WriteAllText($macosLauncherScript, $macosLauncherContent)

# Set executable permission attribute (will only matter when extracted on macOS)
(Get-Item $macosLauncherScript).Attributes = 'Archive'
Write-Host "macOS Steam launcher script created at: $macosLauncherScript" -ForegroundColor Green

# Define output directory where all final builds will be stored
$outputDir = "H:\KrissJourney\SteamRuntimeBuild\output"
New-Item -Path $outputDir -ItemType Directory -Force | Out-Null

# Create archives for each platform
$runtimes | ForEach-Object {
    $runtime = $_.Rid
    
    # Create archive with appropriate files
    $publishDir = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish"
    
    if ($runtime -eq "win-x64") {
        # For Windows, create regular ZIP for standard distribution
        $compressedFile = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish\Kriss-$runtime.zip"
        Write-Host "Creating archive for $runtime..." -ForegroundColor Cyan
        
        # Windows only needs the exe
        $filesToInclude = Join-Path -Path $publishDir -ChildPath "Kriss.exe"
        
        # Create the ZIP archive
        Compress-Archive -Path $filesToInclude -DestinationPath $compressedFile -Force
        Write-Host "Archive created: $compressedFile" -ForegroundColor Green
    }
    else {
        # For macOS, skip regular ZIP and create only Steam-specific version
        $steamMacZip = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish\Kriss-Steam-macOS.zip"
        Write-Host "Creating Steam macOS archive..." -ForegroundColor Cyan
        
        # macOS needs both the executable and the launcher script
        $filesToInclude = @(
            (Join-Path -Path $publishDir -ChildPath "Kriss"),
            (Join-Path -Path $publishDir -ChildPath "krissLauncher.sh")
        )
        
        # Create the ZIP archive directly with the Steam name
        Compress-Archive -Path $filesToInclude -DestinationPath $steamMacZip -Force
        Write-Host "Steam macOS build created: $steamMacZip" -ForegroundColor Green
    }
}

# Create a symbolic link or copy the Linux ZIP to the release folder for consistency
$steamLinuxZip = "H:\KrissJourney\SteamRuntimeBuild\output\Kriss-Steam-Linux.zip"
$targetLinuxZip = "H:\KrissJourney\Kriss\bin\Release\net8.0\Kriss-linux-x64.zip"

if (Test-Path $steamLinuxZip) {
    Write-Host "Copying Steam Linux build to release folder..." -ForegroundColor Cyan
    Copy-Item -Path $steamLinuxZip -Destination $targetLinuxZip -Force
    Write-Host "Steam Linux build copied to: $targetLinuxZip" -ForegroundColor Green
}
else {
    Write-Host "Steam Linux build not found at: $steamLinuxZip" -ForegroundColor Red
}

# Move all ZIP archives to the output directory
Write-Host "Moving platform ZIP archives to output directory..." -ForegroundColor Cyan

# Move Windows ZIP to output directory
$windowsSourceZip = "H:\KrissJourney\Kriss\bin\Release\net8.0\win-x64\publish\Kriss-win-x64.zip"
$windowsTargetZip = Join-Path -Path $outputDir -ChildPath "Kriss-win-x64.zip"

if (Test-Path $windowsSourceZip) {
    Write-Host "Moving Windows build to output directory..." -ForegroundColor Cyan
    Copy-Item -Path $windowsSourceZip -Destination $windowsTargetZip -Force
    Write-Host "Windows build moved to: $windowsTargetZip" -ForegroundColor Green
}
else {
    Write-Host "Windows build not found at: $windowsSourceZip" -ForegroundColor Red
}

# Move macOS Steam ZIP to output directory
$macosSourceZip = "H:\KrissJourney\Kriss\bin\Release\net8.0\osx-x64\publish\Kriss-Steam-macOS.zip"
$macosTargetZip = Join-Path -Path $outputDir -ChildPath "Kriss-Steam-macOS.zip"

if (Test-Path $macosSourceZip) {
    Write-Host "Moving Steam macOS build to output directory..." -ForegroundColor Cyan
    Copy-Item -Path $macosSourceZip -Destination $macosTargetZip -Force
    Write-Host "Steam macOS build moved to: $macosTargetZip" -ForegroundColor Green
}
else {
    Write-Host "Steam macOS build not found at: $macosSourceZip" -ForegroundColor Red
}

Write-Host "Packaging completed for all platforms." -ForegroundColor Yellow
Write-Host "All platform builds are available in: $outputDir" -ForegroundColor Green
Write-Host " " -ForegroundColor White
Write-Host "STEAM PLATFORM DEPLOYMENT GUIDE:" -ForegroundColor Cyan
Write-Host "-----------------------------" -ForegroundColor Cyan
Write-Host "For Windows: Use Kriss-win-x64.zip and set launch option to: Kriss.exe" -ForegroundColor White
Write-Host "For Linux/Steam Deck: Use Kriss-Steam-Linux.zip and set launch option to: ./krissLauncher.sh" -ForegroundColor White
Write-Host "For macOS: Use Kriss-Steam-macOS.zip and set launch option to: ./krissLauncher.sh" -ForegroundColor White
Write-Host "-----------------------------" -ForegroundColor Cyan