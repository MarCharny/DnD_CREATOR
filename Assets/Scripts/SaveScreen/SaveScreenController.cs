using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class SaveScreenController : MonoBehaviour
{
    [SerializeField] SaveScreenView m_view;
    [SerializeField] private ConfirmPanel m_confirmPanel;
    [SerializeField] private string m_editorScreenName;
    [SerializeField] private bool isCharacter = false;

    private SaveSystemLoaderMap m_map;
    private SaveSystemLoaderCharacter m_character;

    private void Awake()
    {
        if (isCharacter)
        {
            m_character = new SaveSystemLoaderCharacter();
            m_view.LoadData(m_character.FindAllSaves());
        }
        else
        {
            m_map = new SaveSystemLoaderMap();
            m_view.LoadData(m_map.FindAllSaves());
        }
    }

    private void OnEnable()
    {
        foreach (SaveSlotController slot in m_view.m_slots)
        {
            slot.m_button.onClick.AddListener(() => OnSlotClicked(slot));
        }

        m_confirmPanel.OnConfirm += OnCharacterConfirmed;
        m_confirmPanel.OnCancel += OnCharacterCreationCanceled;
    }

    private void OnDisable()
    {
        foreach (var slot in m_view.m_slots)
        {
            slot.m_button.onClick.RemoveAllListeners();
        }

        m_confirmPanel.OnConfirm -= OnCharacterConfirmed;
        m_confirmPanel.OnCancel -= OnCharacterCreationCanceled;
    }

    private void OnSlotClicked(SaveSlotController slot)
    {
        if (slot.isFilled)
        {
            Load(slot);
        }
        else
        {
            m_confirmPanel.gameObject.SetActive(true);
            m_confirmPanel.ClearInput();
        }
    }

    private void Load(SaveSlotController slot)
    {
        string characterName = slot.GetText();
        if ( (isCharacter))
        {
            m_character = new SaveSystemLoaderCharacter();
        }
        else
        {
            m_map = new SaveSystemLoaderMap();
        }

        if (isCharacter)
        {
            Character character = m_character.LoadData(characterName);
            CharacterDataHolder.Instance.CurrentCharacter = character;

            if (character != null)
            {
                SceneManager.LoadScene(m_editorScreenName);
            }
            else
            {
                Debug.LogError($"Character with name {characterName} not found!");
            }
        }
        else
        {
            Location map = m_map.LoadData(characterName);
            MapDataHolder.Instance.CurrentMap = map;
            if (map != null)
            {
                SceneManager.LoadScene(m_editorScreenName);
            }
            else
            {
                Debug.LogError($"Character with name {characterName} not found!");
            }
        }
    }

    private void OnCharacterConfirmed(string name, Vector2 position)
    {
        if (isCharacter)
        {
            Character newCharacter = new Character
            {
                characterName = name,
                level = 1,
                age = 20,
                characterClassIdx = 0,
                characterRaceIdx = 0,
                history = "",
                appearance = "",
                clothes = "",
                pathToFullBodyImage = "",
                pathToAvatarImage = "",
                sex = 0
            };
            CharacterDataHolder.Instance.CurrentCharacter = newCharacter;
            SceneManager.LoadScene(m_editorScreenName);
        }
        else
        {
            Location newMap = new Location
            {
                locationName = name,
                parentMapName = "",
                positionOnParentMap = Vector2.zero,
                commonDesc = "",
                styleDesc  = "",
                objectsDesc = "",
                subLocations = new List<Location>()
            };
            MapDataHolder.Instance.CurrentMap = newMap;
            SceneManager.LoadScene(m_editorScreenName);
        }
        m_confirmPanel.gameObject.SetActive(false);
    }

    private void OnCharacterCreationCanceled()
    {
        m_confirmPanel.gameObject.SetActive(false);
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
