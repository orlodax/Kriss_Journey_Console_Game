using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using KrissJourney.Kriss.Models;

namespace KrissJourney.Kriss.Services;

public class StatusManager
{
    /// <summary>
    /// Gets the path to the application data directory (location of status file).
    /// </summary>
    protected virtual string AppDataPath { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the status object that represents the game save.
    /// </summary>
    protected virtual Status Status { get; private set; } = new();

    private readonly string _localStatusFilePath;
    private const string CloudFileName = "status.json";

    public StatusManager()
    {
        string baseAppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KrissJourney");

        // Include Steam player ID in the path if available
        if (SteamManager.Initialized && SteamManager.GetSteamId() != 0)
            AppDataPath = Path.Combine(baseAppDataPath, SteamManager.GetSteamId().ToString());
        else
            AppDataPath = Path.Combine(baseAppDataPath, "default");

        if (!Directory.Exists(AppDataPath))
            Directory.CreateDirectory(AppDataPath);

        _localStatusFilePath = Path.Combine(AppDataPath, CloudFileName);

        LoadStatus();
    }

    private void LoadStatus()
    {
        if (SteamManager.Initialized && SteamManager.CloudFileExists(CloudFileName))
        {
            Debug.WriteLine($"Attempting to load '{CloudFileName}' from Steam Cloud.");
            byte[] cloudData = SteamManager.ReadCloudFile(CloudFileName);
            if (cloudData != null && cloudData.Length > 0) // Ensure data is not null and not empty
            {
                try
                {
                    Status = JsonSerializer.Deserialize<Status>(cloudData, JsonHelper.Options);
                    // Sync to local file
                    File.WriteAllBytes(_localStatusFilePath, cloudData);
                    Debug.WriteLine($"'{CloudFileName}' loaded from Steam Cloud and synced to local: '{_localStatusFilePath}'.");

                    return; // Exit early since we successfully loaded from cloud
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing '{CloudFileName}' from Steam Cloud: {ex.Message}. Falling back to local file.");
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Error writing cloud status to local file '{_localStatusFilePath}': {ex.Message}.");
                }
            }
            else if (cloudData != null) // cloudData.Length == 0
                Debug.WriteLine($"'{CloudFileName}' in Steam Cloud is empty. Treating as no definitive cloud save.");
            else // cloudData == null (ReadCloudFile returned null due to error or file truly not existing)
                Debug.WriteLine($"Failed to read '{CloudFileName}' from Steam Cloud. Falling back to local.");
        }

        if (!File.Exists(_localStatusFilePath))
        {
            Debug.WriteLine($"No local status file found ('{_localStatusFilePath}') and no/invalid cloud save. Initializing new status.");
            Status = new Status();
            // Save this new empty status locally and to cloud immediately if Steam is initialized.
            SaveInternal(); // This will save locally and attempt cloud if Steam is up.
        }
        else
        {
            Debug.WriteLine($"Attempting to load status from local file: '{_localStatusFilePath}'.");
            try
            {
                string localJson = File.ReadAllText(_localStatusFilePath);
                if (!string.IsNullOrWhiteSpace(localJson)) // Ensure local file is not empty
                {
                    Status = JsonSerializer.Deserialize<Status>(localJson, JsonHelper.Options);
                    Debug.WriteLine($"Status loaded from local file: '{_localStatusFilePath}'.");

                    // If Steam is initialized and we loaded locally (meaning cloud was missing, empty, or failed to load),
                    // attempt to upload this local version to the cloud.
                    if (SteamManager.Initialized)
                    {
                        Debug.WriteLine($"Attempting to sync local '{_localStatusFilePath}' to Steam Cloud ('{CloudFileName}').");
                        byte[] localData = File.ReadAllBytes(_localStatusFilePath);
                        if (SteamManager.WriteCloudFile(CloudFileName, localData))
                            Debug.WriteLine($"Local '{_localStatusFilePath}' synced to Steam Cloud ('{CloudFileName}').");
                        else
                            Debug.WriteLine($"Failed to sync local '{_localStatusFilePath}' to Steam Cloud ('{CloudFileName}').");
                    }
                }
                else
                {
                    Debug.WriteLine($"Local status file '{_localStatusFilePath}' is empty. Initializing new status.");
                    Status = new Status();
                    // Optionally save this new empty status if Steam is up, as it implies no valid save anywhere.
                    if (SteamManager.Initialized)
                        SaveInternal();
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Error deserializing status from local file '{_localStatusFilePath}': {ex.Message}. Initializing new status.");
                Status = new Status();
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Error reading local status file '{_localStatusFilePath}': {ex.Message}. Initializing new status.");
                Status = new Status();
            }
        }
    }

    private void SaveInternal()
    {
        byte[] statusData;
        try
        {
            statusData = JsonSerializer.SerializeToUtf8Bytes(Status, JsonHelper.Options);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Error serializing status for saving: {ex.Message}");
            return;
        }

        // Save locally
        try
        {
            File.WriteAllBytes(_localStatusFilePath, statusData);
            Debug.WriteLine($"Status saved to local file: '{_localStatusFilePath}'.");
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Error writing status to local file '{_localStatusFilePath}': {ex.Message}");
            // Potentially critical error, but we'll still try to save to cloud.
        }

        // Save to Steam Cloud
        if (SteamManager.Initialized)
        {
            if (SteamManager.WriteCloudFile(CloudFileName, statusData))
                Debug.WriteLine($"Status saved to Steam Cloud: '{CloudFileName}'.");
            else
                Debug.WriteLine($"Failed to save status to Steam Cloud: '{CloudFileName}'.");
        }
    }

    public virtual void SaveProgress(int chapterId, int nodeId)
    {
        bool changed = false;
        if (Status.VisitedNodes.TryGetValue(chapterId, out List<int> visitedNodes))
        {
            if (!visitedNodes.Contains(nodeId))
            {
                visitedNodes.Add(nodeId);
                changed = true;
            }
        }
        else
        {
            Status.VisitedNodes[chapterId] = [nodeId];
            changed = true;
        }

        if (changed)
            SaveInternal();
    }

    public virtual bool HasVisitedNodes()
    {
        return Status.VisitedNodes.Count != 0;
    }

    public virtual int GetLastChapterId()
    {
        if (Status.VisitedNodes.Count == 0)
            return 1;

        return Status.VisitedNodes.Keys.Max();
    }

    public virtual bool IsNodeVisited(int chapterId, int nodeId)
    {
        if (Status.VisitedNodes.TryGetValue(chapterId, out List<int> visitedNodes))
            return visitedNodes.Contains(nodeId);

        return false;
    }

    public virtual void AddItemToInventory(string item)
    {
        Status.Inventory ??= [];
        if (!Status.Inventory.Contains(item))
        {
            Status.Inventory.Add(item);
            SaveInternal();
        }
    }

    public virtual bool IsItemInInventory(string item)
    {
        if (Status.Inventory == null)
            return false;

        return Status.Inventory.Contains(item);
    }
}