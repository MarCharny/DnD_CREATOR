using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MapEditorPanelView : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_common;
    [SerializeField] private TMP_InputField m_style;
    [SerializeField] private TMP_InputField m_objects;

    [SerializeField] private TMP_Text m_genQuest;
    [SerializeField] private TMP_Text m_genChest;

    [SerializeField] private Button m_generateBtn;
    [SerializeField] private Button m_genQuestBtn;
    [SerializeField] private Button m_genChestBtn;



    public event Action onGenerateMap;
    public event Action onGenQuest;
    public event Action onGenChest;

    private void Awake()
    {
        m_common.ActivateInputField();
        m_style.ActivateInputField();
        m_objects.ActivateInputField();
        m_genChestBtn.onClick.AddListener(HandleGenChestBtnClick);
        m_genQuestBtn.onClick.AddListener(HandleGenQuestBtnClick);
        m_generateBtn.onClick.AddListener(HandleGenerateBtnClick);
    }

    private void OnDestroy()
    {
        m_genChestBtn.onClick.RemoveAllListeners();
        m_genQuestBtn.onClick.RemoveAllListeners();
        m_generateBtn.onClick.RemoveAllListeners();
    }

    private void HandleGenerateBtnClick()
    {
        onGenerateMap?.Invoke();
    }

    private void HandleGenChestBtnClick()
    {
        onGenChest?.Invoke();
    }

    private void HandleGenQuestBtnClick()
    {
        onGenQuest?.Invoke();
    }


    public string GetCommonDesc()
    {
        return m_common.text;
    }

    public string GetStyleDesc() 
    { 
        return m_style.text;
    }

    public string GetObjectsDesc()
    {
        return m_objects.text;
    }

    public void SetCommonDesc(string desc)
    {
        m_common.text = desc;
    }
    public void SetStyleDesc(string desc)
    {
        m_style.text = desc;
    }
    public void SetObjectsDesc(string desc)
    {
        m_objects.text = desc;
    }



    public void SetGeneratedTextChest(string text)
    {
        m_genChest.text = text;
    }

    public void SetGeneratedTextQuest(string quest)
    {
        m_genQuest.text = quest;
    }
}
