using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string urlToOpen = "https://dungeonsanddragons.ru/bookfull/5ed/5e%20Players%20Handbook%20-%20%D0%9A%D0%BD%D0%B8%D0%B3%D0%B0%20%D0%B8%D0%B3%D1%80%D0%BE%D0%BA%D0%B0%20RUS.pdf";

    public void OpenDnDLink()
    {
        if (!string.IsNullOrEmpty(urlToOpen))
        {
            Application.OpenURL(urlToOpen);
        }
        else
        {
            Debug.LogWarning("URL не указан!");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadSaveCharacters()
    {
        LoadScene("SaveScreenCharacter");
    }

    public void LoadSaveMaps()
    {
        LoadScene("SaveScreenMap");
    }

    private void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not found!");
        }
    }
}
