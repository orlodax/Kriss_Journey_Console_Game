global using static System.Console;
using System.Diagnostics;
using System.Threading.Tasks;
using KrissJourney.Kriss.Classes;

Title = "KRISS' JOURNEY";

#region Steam API Initialization
SteamManager.Initialize(() =>
{
    // This will be called when the application quits
    Debug.WriteLine("Game shutting down - cleaning up Steam resources");
});

// Set up a simple callback runner because the game doesn't have a dedicated update loop
_ = Task.Run(async () =>
{
    while (SteamManager.Initialized)
    {
        SteamManager.RunCallbacks();
        await Task.Delay(15);
    }
});

// Register an event to handle application exit
System.AppDomain.CurrentDomain.ProcessExit += (s, e) =>
{
    SteamManager.Shutdown();
};
#endregion

DataLayer.Init();