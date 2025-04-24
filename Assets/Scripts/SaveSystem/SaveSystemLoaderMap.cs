using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemLoaderMap : SaveSystemLoader<Location>
{
    private const string MapsBasePath = "/maps/";
    private const string DataSubPath = "/data/";
    private const string JsonExt = ".json";
    private const string MediaSubPath = "/media/";
    private const string PngExt = ".png";

    public Location LoadData(string mapName)
    {
        string filePath = GetMapFilePath(mapName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Location>(json);
        }

        Debug.LogError($"Map data file not found at {filePath}");
        return null;
    }

    public (string name, string imagePath)[] FindAllSaves()
    {
        string fullPath = Application.persistentDataPath + MapsBasePath;

        if (!Directory.Exists(fullPath))
        {
            return new (string, string)[0];
        }

        var mapDirs = Directory.GetDirectories(fullPath);
        var result = new List<(string, string)>();

        foreach (var dir in mapDirs)  
        {
            string mapName = new DirectoryInfo(dir).Name;
            string jsonPath = Path.Combine(dir, DataSubPath, mapName + JsonExt);
            string fullJsonPath = fullPath + mapName + jsonPath;

            if (File.Exists(fullJsonPath))
            {
                try
                {
                    string json = File.ReadAllText(fullJsonPath);
                    var data = JsonUtility.FromJson<Location>(json);
                    result.Add((data.locationName, data.pathToMapImage));
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error loading map data from {jsonPath}: {e.Message}");
                }
            }
        }

        return result.ToArray();
    }

    public string GetMapImagePath(string mapName)
    {
        return Application.persistentDataPath + MapsBasePath + mapName + MediaSubPath + mapName + PngExt;
    }

    public string GetMapFilePath(string mapName)
    {
        return Application.persistentDataPath + MapsBasePath + mapName + DataSubPath + mapName + JsonExt;
    }
}
