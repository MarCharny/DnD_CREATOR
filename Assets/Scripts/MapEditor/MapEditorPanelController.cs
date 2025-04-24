using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapEditorPanelController : MonoBehaviour
{
    [SerializeField] private MapEditorPanelView view; 

    private AIManagerInterface AIManager;
    private MapPromptBuilder Builder;

    public event Action<Texture2D> onMapLoaded;
    public event Action<bool> ToggleGenerate;
    public event Action onError;
    
    private void Awake()
    {
        AIManager = new AIReplicateManager();
        Builder = new MapPromptBuilder();
    }

    private void OnEnable()
    {
        view.onGenChest += GenerateChest;
        view.onGenQuest += GenerateQuest;
        view.onGenerateMap += GenerateMap;
    }

    private void OnDisable()
    {
        view.onGenQuest -= GenerateQuest;
        view.onGenChest -= GenerateChest;
        view.onGenerateMap -= GenerateMap;
    }

    public void SetData(Location data)
    {
        view.SetCommonDesc(data.commonDesc);
        view.SetStyleDesc(data.styleDesc);
        view.SetObjectsDesc(data.objectsDesc);
    }

    public void FillData(Location data)
    {
        data.commonDesc = view.GetCommonDesc();
        data.styleDesc = view.GetStyleDesc();
        data.objectsDesc = view.GetObjectsDesc();
    }

    public async void GenerateQuest()
    {
        ToggleGenerate?.Invoke(true);
        string prompt = "\r\nCreate a vivid Dungeons & Dragons quest description including these elements:\r\n1. Quest Name: [Generate creative name]\r\n2. Hook: Why the party should care (15-25 words)\r\n3. Destination: Where they must go (specific location)\r\n4. Objective: Clear goal (retrieve, destroy, protect, investigate)\r\n5. Challenges: 3 potential encounters (combat, puzzle, social)\r\n6. Stakes: What happens if they fail\r\n7. Reward: Worthy compensation\r\n\r\nFormat concisely in 150-200 tokens. Focus on dark fantasy atmosphere with unexpected twists. The quest should be suitable for level 3-5 characters";
        string result = await AIManager.GenerateTextAsync(prompt);
        string final = CleanAnswerText(result);

        if (!string.IsNullOrEmpty(final))
        { 
            view.SetGeneratedTextQuest(final);
        }
        else
        {
            onError?.Invoke();
        }
        ToggleGenerate?.Invoke(false);
    }

    public async void GenerateChest()
    {
        ToggleGenerate?.Invoke(true);   
        string prompt = "\r\nGenerate a vivid D&D 5e treasure chest contents description with:\r\n1. **Gold**: 1d100 x 10 (specify exact amount)\r\n2. **Magic Items**: 1-2 items (rarity: uncommon/rare, include attunement if needed)\r\n3. **Spell Components**: 1d4 types (e.g. \"bat guano for Fireball\")\r\n4. **Curiosities**: 1 strange non-magical item (e.g. \"elf king's love letter\")\r\n5. **Hidden Surprise**: 10% chance for a secret (trap/fake bottom/cursed item)";
        string result = await AIManager.GenerateTextAsync(prompt);
        string final = CleanAnswerText(result);

        if (!string.IsNullOrEmpty(final))
        {
            view.SetGeneratedTextChest(final);
        }
        else
        {
            onError?.Invoke();
        }
        ToggleGenerate?.Invoke(false);
    }

    public async void GenerateMap()
    {
        ToggleGenerate?.Invoke(true);
        Builder.Reset();
        string prompt = Builder.AddBasic()
            .AddCommon(view.GetCommonDesc())
            .AddStyle(view.GetStyleDesc())
            .AddObjects(view.GetObjectsDesc())
            .Build();

        Texture2D mapImage = await AIManager.GenerateImageAsync(prompt, ImageType.Map);
        if (mapImage != null)
        {
            onMapLoaded?.Invoke(mapImage);
        }
        else
        {
            onError?.Invoke();
        }
        ToggleGenerate?.Invoke(false);
    }

    public void FillLocationData(Location data)
    {
        data.commonDesc = view.GetCommonDesc();
        data.objectsDesc = view.GetObjectsDesc();
        data.styleDesc = view.GetStyleDesc();
    }

    public void SetLocationEditorData(Location data)
    {
        view.SetCommonDesc(data.commonDesc);
        view.SetStyleDesc(data.styleDesc);
        view.SetObjectsDesc(data.objectsDesc);
    }

    private string CleanAnswerText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "Not generated";

        string processed = text
            .Replace("**", "")    
            .Replace("*", "")     
            .Replace("#", "")     
            .Replace("\"", "")    
            .Replace("\\n", "\n") 
            .Trim();

        processed = System.Text.RegularExpressions.Regex.Replace(
            processed,
            @"\n+",
            "\n"
        );

        processed = System.Text.RegularExpressions.Regex.Replace(
            processed,
            @"\s+",
            " "
        );

        return processed.Trim();
    }
}
