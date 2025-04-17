using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleCtrl : MonoBehaviour
{
    [SerializeField] public int value;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.black;
    [SerializeField] private TextMeshProUGUI text;

    private Toggle obj;

    private void Awake()
    {
        obj = GetComponent<Toggle>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        obj.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (text != null)
        {
            text.color = isOn ? selectedColor : normalColor;
        }
    }

    private void OnDestroy()
    {
        obj.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

}
