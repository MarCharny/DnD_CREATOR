using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_name;
    [SerializeField] private RawImage m_img;
    [SerializeField] public Button m_button;

    public bool isFilled {get; set;}


    private void OnEnable()
    {
        isFilled = false;
    }
        
    private void Update()
    {
        if (isFilled)
        {
            m_name.gameObject.SetActive(true);
            m_img.gameObject.SetActive(true);
        }
        else
        {
            m_name.gameObject.SetActive(false);
            m_img.gameObject.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        m_name.text = text;
    }

    public void SetImage(Texture2D image)
    {
        m_img.texture = image;
    }

    public string GetText()
    {
        return m_name.text;
    }
}
