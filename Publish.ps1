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

$runtimes | ForEach-Object -Parallel {
    $runtime = $_.rid
    
    # Compress the published file
    $fileName = "Kriss"
    if ($runtime -eq "win-x64") {
        $fileName += ".exe"
    }

    $publishedFile = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish\$fileName"
    $compressedFile = "H:\KrissJourney\Kriss\bin\Release\net8.0\$runtime\publish\Kriss-$runtime.zip"
    
    Write-Host "Compressing $runtime executable..." -ForegroundColor Cyan
    Compress-Archive -Path $publishedFile -DestinationPath $compressedFile -Force
    Write-Host "Compression completed: $compressedFile" -ForegroundColor Green
}

Write-Host "Compressing completed for all platforms." -ForegroundColor Yellow