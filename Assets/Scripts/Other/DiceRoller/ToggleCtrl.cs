using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleCtrl : MonoBehaviour
{
    [SerializeField] public int m_value;

    [SerializeField] private Color m_normalColor = Color.white;
    [SerializeField] private Color m_selectedColor = Color.black;
    [SerializeField] private TextMeshProUGUI m_text;

    private Toggle m_obj;

    private void Awake()
    {
        m_obj = GetComponent<Toggle>();
        m_text = GetComponentInChildren<TextMeshProUGUI>();

        m_obj.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (m_text != null)
        {
            m_text.color = isOn ? m_selectedColor : m_normalColor;
        }
    }

    private void OnDestroy()
    {
        m_obj.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

}
