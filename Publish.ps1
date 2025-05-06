$runtimes = @(
    @{ Name = "Windows x64"; Rid = "win-x64" },
    @{ Name = "Linux x64"; Rid = "linux-x64" },
    @{ Name = "OSX x64"; Rid = "osx-x64" }
)

$runtimes | ForEach-Object -Parallel {
    $runtime = $_.rid
    Write-Host "Publishing for $runtime..." -ForegroundColor Cyan
    $process = Start-Process -FilePath "dotnet" -ArgumentList "publish -c Release -r $runtime -p:PublishSingleFile=true --self-contained true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName=Kriss" -WorkingDirectory "H:\KrissJourney\Kriss" -NoNewWindow -PassThru -RedirectStandardOutput "$runtime-output.log"
    $process.WaitForExit()
    
    $output = Get-Content -Path "$runtime-output.log" -Raw
    Write-Host $output
    Write-Host "Publish completed with exit code: $($process.ExitCode)" -ForegroundColor Green
}

Write-Host "Publishing completed for all platforms." -ForegroundColor Yellow

# Copy the launcher script to Linux and macOS publish directories
$runtimes | ForEach-Object {
    $runtime = $_.rid
    
    # Only copy for Linux and macOS
    if ($runtime -eq "linux-x64" -or $runtime -eq "osx-x64") {
        $publishDir = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish"
        $launcherScript = "H:\KrissJourney\krissLauncher.sh"
        
        Write-Host "Copying launcher script to $runtime publish directory..." -ForegroundColor Cyan
        Copy-Item -Path $launcherScript -Destination $publishDir -Force
        
        # Set executable permission attribute (will only matter when extracted on Unix)
        $targetFile = Join-Path -Path $publishDir -ChildPath "krissLauncher.sh"
        if (Test-Path $targetFile) {
            # Mark the file as executable in metadata
            # This doesn't actually set Unix permissions but helps indicate it should be executable
            (Get-Item $targetFile).Attributes = 'Archive'
            Write-Host "Launcher script copied to $targetFile" -ForegroundColor Green
        }
        else {
            Write-Host "Failed to copy launcher script to $runtime publish directory" -ForegroundColor Red
        }
    }
}

$runtimes | ForEach-Object -Parallel {
    $runtime = $_.rid
    
    # Create archive with appropriate files
    $publishDir = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish"
    $compressedFile = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish\Kriss-$runtime.zip"
    
    Write-Host "Creating archive for $runtime..." -ForegroundColor Cyan
    
    # Different files to include based on platform
    if ($runtime -eq "win-x64") {
        # Windows only needs the exe
        $filesToInclude = Join-Path -Path $publishDir -ChildPath "Kriss.exe"
    }
    else {
        # Linux and macOS need both the executable and the launcher script
        $filesToInclude = @(
            (Join-Path -Path $publishDir -ChildPath "Kriss"),
            (Join-Path -Path $publishDir -ChildPath "krissLauncher.sh")
        )
    }
    
    # Create the ZIP archive
    Compress-Archive -Path $filesToInclude -DestinationPath $compressedFile -Force
    
    Write-Host "Archive created: $compressedFile" -ForegroundColor Green
}

Write-Host "Packaging completed for all platforms." -ForegroundColor Yellow