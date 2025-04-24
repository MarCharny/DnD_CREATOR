using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    public string locationName;
    public string style;
    public string commonDesc;
    public string styleDesc;
    public string objectsDesc;
    public string pathToMapImage;
    public string parentMapName;
    public Vector2 positionOnParentMap;
    public List<Location> subLocations;
}

[System.Serializable]
public class Character
{
    public string characterName;
    public uint level;
    public uint age;
    public int characterClassIdx;
    public int characterRaceIdx;
    public string history;
    public string appearance;
    public string clothes;
    public string pathToFullBodyImage;
    public string pathToAvatarImage;
    public int sex;
}
