using static UnityEngine.Rendering.DebugUI;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CustomOptionData
{
    public string text;
    public Sprite icon;
}

public class CustomDropDown : TMPro.TMP_Dropdown
{
    [SerializeField] private List<CustomOptionData> m_Options = new List<CustomOptionData>();
    [SerializeField] private string defaultSelectedOption;

    protected override void Start()
    {
        base.Start();
        options.Clear();
        foreach (var option in m_Options)
        {
            options.Add(new TMPro.TMP_Dropdown.OptionData(option.text, option.icon));
        }

        if (!string.IsNullOrEmpty(defaultSelectedOption))
        {
            SelectOptionByName(defaultSelectedOption);
        }
    }

    public string GetSelectedOptionText()
    {
        if (value >= 0 && value < m_Options.Count)
        {
            return m_Options[value].text;
        }
        return string.Empty;
    }

    public void SelectOptionByName(string optionName)
    {
        int index = m_Options.FindIndex(option => option.text == optionName);
        if (index >= 0)
        {
            value = index;
            RefreshShownValue();
        }
        else if (m_Options.Count > 0)
        {
            value = 0;
            RefreshShownValue();
        }
    }
}