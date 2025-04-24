using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class CharacterEditorPanelView : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_appearance;
    [SerializeField] private TMP_InputField m_clothing;
    [SerializeField] private Button m_generateBtn;
    [SerializeField] private ToggleGroup m_sex;
    [SerializeField] private TMP_Dropdown m_class;
    [SerializeField] private TMP_Dropdown m_race;
    [SerializeField] private TMP_InputField m_age;
    [SerializeField] private TMP_InputField m_level;
    [SerializeField] private TMP_InputField m_history;

    public event Action onGenerate;

    private void Awake()
    {
        m_generateBtn.onClick.AddListener(HandleGenerateBtnClick);
    }

    private void OnDestroy()
    {
        m_generateBtn.onClick.RemoveAllListeners();
    }
    private void HandleGenerateBtnClick()
    {
        onGenerate?.Invoke();
    }

    public string GetAppearanceDesc()
    {
        return m_appearance.text;
    }

    public string GetClothingDesc()
    {
        return m_appearance.text;
    }

    public void SetAppearanceDesc(string desc)
    {
        m_appearance.text = desc;
    }

    public void SetClothingDesc(string desc)
    {
        m_clothing.text = desc;
    }

    public int GetSex()
    {
        return m_sex.GetFirstActiveToggle().GetComponent<ToggleCtrl>().m_value;
    }

    public void SetSex(int value)
    {
        var toggles = m_sex.GetComponentsInChildren<Toggle>();

        foreach (var toggle in toggles)
        {
            ToggleCtrl ctrl = toggle.GetComponent<ToggleCtrl>();
            if (ctrl.m_value == value)
            {
                toggle.isOn = true;
                break;
            }
        }
    }

    public int GetRace()
    {
        return m_race.value;
    }

    public void SetRace(int val)
    {
        m_race.value = val;
    }

    public string GetRaceText()
    {
        return m_race.captionText.text;
    }

    public int GetClass()
    {
        return m_class.value;
    }

    public void SetClass(int val)
    {
        m_class.value = val;
    }

    public string GetClassText()
    {
        return m_class.captionText.text;
    }

    public uint GetAge()
    {
        return uint.Parse(m_age.text);
    }

    public void SetAge(uint desc)
    {
        m_age.text = desc.ToString();
    }

    public uint GetLevel()
    {
        return uint.Parse(m_level.text);
    }

    public void SetLevel(uint level)
    {
        m_level.text = level.ToString();
    }

    public string GetHistory()
    {
        return m_history.text;
    }

    public void SetHistory(string level)
    {
        m_history.text = level;
    }
}
