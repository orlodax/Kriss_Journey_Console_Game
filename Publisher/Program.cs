// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

PublishForWindowsX64();
PublishForLinuxX64();
PublishForOsxX64();
Console.WriteLine("Publishing completed for all platforms.");
Console.WriteLine("Press any key to exit...");

void PublishForWindowsX64()
{
    Console.WriteLine("Publishing for Windows x64...");
    Process process = new();
    process.StartInfo.FileName = "dotnet";
    process.StartInfo.Arguments = "publish -c Release -r win-x64 --self-contained true /p:DebugType=None /p:DebugSymbols=false";
    process.StartInfo.WorkingDirectory = @"h:\KrissJourney\Kriss";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    Console.WriteLine(output);
    Console.WriteLine($"Publish completed with exit code: {process.ExitCode}");
}

void PublishForLinuxX64()
{
    Console.WriteLine("Publishing for Linux x64...");
    Process process = new();
    process.StartInfo.FileName = "dotnet";
    process.StartInfo.Arguments = "publish -c Release -r linux-x64 --self-contained true /p:DebugType=None /p:DebugSymbols=false";
    process.StartInfo.WorkingDirectory = @"h:\KrissJourney\Kriss";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    Console.WriteLine(output);
    Console.WriteLine($"Publish completed with exit code: {process.ExitCode}");
}

void PublishForOsxX64()
{
    Console.WriteLine("Publishing for OSX x64...");
    Process process = new();
    process.StartInfo.FileName = "dotnet";
    process.StartInfo.Arguments = "publish -c Release -r osx-x64 --self-contained true /p:DebugType=None /p:DebugSymbols=false";
    process.StartInfo.WorkingDirectory = @"h:\KrissJourney\Kriss";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    Console.WriteLine(output);
    Console.WriteLine($"Publish completed with exit code: {process.ExitCode}");
}