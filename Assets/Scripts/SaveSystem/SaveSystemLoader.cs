using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SaveSystemLoader<T>
{
    T LoadData(string filePath);
    (string name, string imagePath)[] FindAllSaves();
}