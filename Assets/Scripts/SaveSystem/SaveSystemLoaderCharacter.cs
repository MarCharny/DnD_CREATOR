using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystemLoaderCharacter : SaveSystemLoader<Character>
{
    private const string CharactersBasePath = "/characters/";
    private const string DataSubPath = "/data/";
    private const string JsonExt = ".json";
    private const string MediaSubPath = "/media/";
    private const string PngExt = ".png";

    public Character LoadData(string characterName)
    {
        string filePath = GetCharacterFilePath(characterName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Character>(json);
        }

        Debug.LogError($"Character data file not found at {filePath}");
        return null;
    }

    public (string name, string imagePath)[] FindAllSaves()
    {
        string fullPath = Application.persistentDataPath + CharactersBasePath;

        if (!Directory.Exists(fullPath))
        {
            return new (string, string)[0];
        }

        var characterDirs = Directory.GetDirectories(fullPath);
        var result = new List<(string, string)>();

        foreach (var dir in characterDirs)
        {
            string characterName = new DirectoryInfo(dir).Name;
            string jsonPath = Path.Combine(dir, DataSubPath, characterName + JsonExt);
            string fullJsonPath = fullPath + characterName + jsonPath;
            if (File.Exists(fullJsonPath))
            {
                try
                {
                    string json = File.ReadAllText(fullJsonPath);
                    var data = JsonUtility.FromJson<Character>(json);
                    result.Add((data.characterName, data.pathToFullBodyImage));
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error loading character data from {jsonPath}: {e.Message}");
                }
            }
        }

        return result.ToArray();
    }

    public string GetCharacterFilePath(string characterName)
    {
        return Application.persistentDataPath + CharactersBasePath + characterName + DataSubPath + characterName + JsonExt;
    }

    public string GetCharacterAvatarFilePath(string characterName)
    {
        return (Application.persistentDataPath + CharactersBasePath + characterName + MediaSubPath + characterName + "_avatar" + PngExt).Replace(" ", "");
    }

    public string GetCharacterFullBodyFilePath(string characterName)
    {
        return (Application.persistentDataPath + CharactersBasePath + characterName + MediaSubPath + characterName + "_full" + PngExt).Replace(" ", "");
    }

    public string GetCharacterCardFilePath(string characterName)
    {
        return (Application.persistentDataPath + CharactersBasePath + characterName + MediaSubPath + characterName + "_card" + PngExt).Replace(" ", "");
    }
}
