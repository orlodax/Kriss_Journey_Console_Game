function Publish-ForWindowsX64 {
    Write-Host "Publishing for Windows x64..." -ForegroundColor Cyan
    $process = Start-Process -FilePath "dotnet" -ArgumentList "publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName=Kriss" -WorkingDirectory "h:\KrissJourney\Kriss" -NoNewWindow -PassThru -RedirectStandardOutput "win-x64-output.log"
    $process.WaitForExit()
    
    $output = Get-Content -Path "win-x64-output.log" -Raw
    Write-Host $output
    Write-Host "Publish completed with exit code: $($process.ExitCode)" -ForegroundColor Green
    
    # Compress the published file
    $publishedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\win-x64\publish\Kriss.exe"
    $compressedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\win-x64\publish\Kriss-win-x64.zip"
    
    Write-Host "Compressing Windows executable..." -ForegroundColor Cyan
    Compress-Archive -Path $publishedFile -DestinationPath $compressedFile -Force
    Write-Host "Compression completed: $compressedFile" -ForegroundColor Green
}

function Publish-ForLinuxX64 {
    Write-Host "Publishing for Linux x64..." -ForegroundColor Cyan
    $process = Start-Process -FilePath "dotnet" -ArgumentList "publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName=Kriss" -WorkingDirectory "h:\KrissJourney\Kriss" -NoNewWindow -PassThru -RedirectStandardOutput "linux-x64-output.log"
    $process.WaitForExit()
    
    $output = Get-Content -Path "linux-x64-output.log" -Raw
    Write-Host $output
    Write-Host "Publish completed with exit code: $($process.ExitCode)" -ForegroundColor Green
    
    # Compress the published file
    $publishedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\linux-x64\publish\Kriss"
    $compressedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\linux-x64\publish\Kriss-linux-x64.zip"
    
    Write-Host "Compressing Linux executable..." -ForegroundColor Cyan
    Compress-Archive -Path $publishedFile -DestinationPath $compressedFile -Force
    Write-Host "Compression completed: $compressedFile" -ForegroundColor Green
}

function Publish-ForOsxX64 {
    Write-Host "Publishing for OSX x64..." -ForegroundColor Cyan
    $process = Start-Process -FilePath "dotnet" -ArgumentList "publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName=Kriss" -WorkingDirectory "h:\KrissJourney\Kriss" -NoNewWindow -PassThru -RedirectStandardOutput "osx-x64-output.log"
    $process.WaitForExit()
    
    $output = Get-Content -Path "osx-x64-output.log" -Raw
    Write-Host $output
    Write-Host "Publish completed with exit code: $($process.ExitCode)" -ForegroundColor Green
    
    # Compress the published file
    $publishedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\osx-x64\publish\Kriss"
    $compressedFile = "h:\KrissJourney\Kriss\bin\Release\net8.0\osx-x64\publish\Kriss-osx-x64.zip"
    
    Write-Host "Compressing OSX executable..." -ForegroundColor Cyan
    Compress-Archive -Path $publishedFile -DestinationPath $compressedFile -Force
    Write-Host "Compression completed: $compressedFile" -ForegroundColor Green
}

# Main script execution
Publish-ForWindowsX64
Publish-ForLinuxX64
Publish-ForOsxX64

Write-Host "Publishing completed for all platforms." -ForegroundColor Yellow