using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SaveScreenView : MonoBehaviour
{
    [SerializeField] public SaveSlotController[] m_slots;

    public void LoadData((string name, string imagePath)[] data)
    {
        int slotsToFill = Mathf.Min(m_slots.Length, data.Length);

        for (int i = 0; i < slotsToFill; i++)
        {
            SaveSlotController slot = m_slots[i];
            slot.SetText(data[i].name);

            if (!string.IsNullOrEmpty(data[i].imagePath))
            {
                slot.SetImage(LoadImage(data[i].imagePath));
            }

            slot.isFilled = true;
        }
    }

    private Texture2D LoadImage(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + filePath);
                UnityEngine.Object.Destroy(texture);
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading texture: " + e.Message);
            return null;
        }
    }
}
