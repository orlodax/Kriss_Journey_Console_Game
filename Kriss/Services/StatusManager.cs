using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KrissJourney.Kriss.Helpers;
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

    public StatusManager()
    {
        AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KrissJourney");

        // Creates the application data directory and loads the status file if it exists.
        if (!Directory.Exists(AppDataPath))
            Directory.CreateDirectory(AppDataPath);

        string statusFile = Path.Combine(AppDataPath, "status.json");
        if (File.Exists(statusFile))
            Status = JsonSerializer.Deserialize<Status>(File.ReadAllText(statusFile), JsonHelper.Options);
    }
    public virtual void SaveProgress(int chapterId, int nodeId)
    {
        if (Status.VisitedNodes.TryGetValue(chapterId, out List<int> visitedNodes))
        {
            if (!visitedNodes.Contains(nodeId))
                visitedNodes.Add(nodeId);
        }
        else
        {
            Status.VisitedNodes[chapterId] = [nodeId];
        }

        File.WriteAllText(
            Path.Combine(AppDataPath, "status.json"),
            JsonSerializer.Serialize(Status, JsonHelper.Options));
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
        if (!Status.Inventory.Contains(item))
            Status.Inventory.Add(item);
    }

    public virtual bool IsItemInInventory(string item)
    {
        return Status.Inventory.Contains(item);
    }
}