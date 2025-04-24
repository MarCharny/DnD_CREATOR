using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_nameInputField;
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private Button m_cancelButton;

    public event System.Action<string, Vector2> OnConfirm;
    public event System.Action OnCancel;

    private Vector2 m_newPos;

    private void Awake()
    {
        m_confirmButton.onClick.AddListener(Confirm);
        m_cancelButton.onClick.AddListener(Cancel);
    }

    public void SetNewPos(Vector2 pos)
    {
        m_newPos = pos;
    }

    public void ClearInput()
    {
        m_nameInputField.text = string.Empty;
    }

    private void Confirm()
    {
        OnConfirm?.Invoke(m_nameInputField.text, m_newPos);
        m_newPos = Vector2.zero;
    }

    private void Cancel()
    {
        OnCancel?.Invoke();
        m_newPos = Vector2.zero;
    }
}