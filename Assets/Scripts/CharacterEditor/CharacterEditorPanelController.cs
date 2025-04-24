using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CharacterEditorPanelController : MonoBehaviour
{
    [SerializeField] private CharacterEditorPanelView view;
    private AIManagerInterface AIManager;
    private CharacterPromptBuilder Builder;

    public event Action<Texture2D> onLoadFullBodyImage;
    public event Action<Texture2D> onLoadAvatarImage;
    public event Action<bool> onToggleGenerate;
    public event Action onError;

    public void FillData(Character data)
    {
        data.history = view.GetHistory();
        data.age = view.GetAge(); 
        data.level = view.GetLevel();
        data.appearance = view.GetAppearanceDesc();
        data.clothes = view.GetClothingDesc();
        data.characterRaceIdx = view.GetRace();
        data.characterClassIdx = view.GetClass();
        data.sex = view.GetSex();
    }

    public void SetData(Character data)
    {
        view.SetAge(data.age);
        view.SetLevel(data.level);
        view.SetRace(data.characterRaceIdx);
        view.SetClass(data.characterClassIdx);
        view.SetSex(data.sex);
        view.SetAppearanceDesc(data.appearance);
        view.SetHistory(data.history);
        view.SetClothingDesc(data.clothes);
    }

    private void Awake()
    {
        AIManager = new AIReplicateManager();
        Builder = new CharacterPromptBuilder();
    }

    private void OnEnable()
    {
        view.onGenerate += GenerateImages;
    }

    private void OnDisable()
    {
        view.onGenerate -= GenerateImages;
    }

    public string GetClass()
    {
        return view.GetClassText();
    }

    public string GetRace()
    {
        return view.GetRaceText();
    }

    public string GetSex()
    {
        return view.GetSex() == 0 ? "Male" : "Female";
    }

    private async void GenerateImages()
    {
        onToggleGenerate?.Invoke(true);
        Builder.Reset();
        string fullBodyPrompt = Builder.AddBasic(true)
            .AddSex(view.GetSex())
            .AddAge(view.GetAge().ToString())
            .AddRace(view.GetRaceText())
            .AddAppearance(view.GetAppearanceDesc())
            .AddClothing(view.GetClothingDesc())
            .Build();

        Builder.Reset();
        string avatarPrompt = Builder.AddBasic(false)
            .AddSex(view.GetSex())
            .AddAge(view.GetAge().ToString())
            .AddRace(view.GetRaceText())
            .AddAppearance(view.GetAppearanceDesc())
            .AddClothing(view.GetClothingDesc())
            .Build();

        Texture2D avatarImage = await AIManager.GenerateImageAsync(avatarPrompt, ImageType.Avatar);
        if (avatarImage != null)
        {
            onLoadAvatarImage?.Invoke(avatarImage);
        }
        else
        {
            onError?.Invoke();
        }

        Texture2D fullBodyImage = await AIManager.GenerateImageAsync(fullBodyPrompt, ImageType.Fullbody);
        if (fullBodyImage != null)
        {
            onLoadFullBodyImage?.Invoke(fullBodyImage);
        }
        else
        {
            onError?.Invoke();
        }
        onToggleGenerate?.Invoke(false);
    }
}
