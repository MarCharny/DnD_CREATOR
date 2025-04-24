using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SaveSystemData
{
    public void SaveData<T>(T data, string filePath)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
            Debug.Log($"Data saved successfully to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data to {filePath}: {e.Message}");
        }
    }

    public void SaveImage(Texture2D texture, string filePath)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Texture2D textureToSave = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            textureToSave.SetPixels(texture.GetPixels());
            textureToSave.Apply();
            byte[] pngData = ImageConversion.EncodeToPNG(textureToSave);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllBytes(filePath, pngData);
            Debug.Log("Avatar image saved to: " + filePath);
            UnityEngine.Object.Destroy(textureToSave);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save image to {filePath}: {e.Message}");
        }
    }
}
