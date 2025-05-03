using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Steamworks;

namespace KrissJourney.Kriss.Classes;

public static class SteamManager
{
    private static bool s_Initialized = false;
    private static bool s_EverInitialized = false;
    private static Action s_OnAppQuitCallback;

    public static bool Initialized => s_Initialized;

    public static bool Initialize(Action onAppQuitCallback = null)
    {
        if (s_Initialized)
            return true;

        s_OnAppQuitCallback = onAppQuitCallback;

        if (s_EverInitialized)
        {
            // If we've initialized once before, we only need to call Init again
            s_Initialized = SteamAPI.Init();
            return s_Initialized;
        }

        try
        {
            // Check if Steam API DLL files exist
            CheckSteamDllFiles();

            // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the 
            // Steam client and also launches this game again if the User owns it
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                // If RestartAppIfNecessary returns true, we need to quit this instance of the game
                Debug.WriteLine("SteamAPI_RestartAppIfNecessary returned true - quitting");
                Environment.Exit(0);
                return false;
            }

            // Initialize Steamworks
            s_Initialized = SteamAPI.Init();
            if (!s_Initialized)
            {
                Debug.WriteLine("SteamAPI Init failed. Make sure Steam is running and you are running the game through Steam.");
                return false;
            }

            s_EverInitialized = true;
            Debug.WriteLine("Steam API initialized successfully");
            return true;
        }
        catch (DllNotFoundException e)
        {
            Debug.WriteLine($"Steam API DLL not found: {e.Message}");
            Debug.WriteLine("Please make sure the Steam API library files are in your application directory:");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Debug.WriteLine($"- {GetExpectedDllName()} (should be in {AppDomain.CurrentDomain.BaseDirectory})");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Debug.WriteLine("- libsteam_api.so");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Debug.WriteLine("- libsteam_api.dylib");

            s_Initialized = false;
            return false;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Steam initialization error: {e.Message}");
            s_Initialized = false;
            return false;
        }
    }

    private static void CheckSteamDllFiles()
    {
        string expectedDll = GetExpectedDllName();

        if (!string.IsNullOrEmpty(expectedDll) && !File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, expectedDll)))
        {
            Debug.WriteLine($"Warning: Could not find {expectedDll} in the application directory.");
            Debug.WriteLine($"Current directory: {AppDomain.CurrentDomain.BaseDirectory}");
            Debug.WriteLine("Available files:");

            try
            {
                foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
                    Debug.WriteLine($" - {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error listing directory files: {ex.Message}");
            }
        }
    }

    private static string GetExpectedDllName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Environment.Is64BitProcess ? "steam_api64.dll" : "steam_api.dll";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "libsteam_api.so";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "libsteam_api.dylib";

        return string.Empty;
    }

    public static void Shutdown()
    {
        if (!s_Initialized)
            return;

        s_OnAppQuitCallback?.Invoke();
        SteamAPI.Shutdown();
        s_Initialized = false;
    }

    public static void RunCallbacks()
    {
        if (!s_Initialized)
            return;

        // Run Steam client callbacks
        SteamAPI.RunCallbacks();
    }
}