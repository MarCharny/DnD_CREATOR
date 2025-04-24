using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class CharacterEditor : MonoBehaviour
{
    [SerializeField] private RawImage m_fullBodyImage;
    [SerializeField] private RawImage m_avatarImage;

    [SerializeField] private GameObject m_card;
    [SerializeField] private string targetLayerName = "PrefabCapture";

    [SerializeField] private Button m_exit;
    [SerializeField] private Button m_save;
    [SerializeField] private TMP_Text m_name;

    [SerializeField] private CharacterEditorPanelController m_panelController;
    [SerializeField] private GameObject m_errorPanel;

    private SaveSystemData m_saveSystemData = new SaveSystemData();
    private SaveSystemLoaderCharacter m_saveLoader = new SaveSystemLoaderCharacter();

    Character m_data;

    private void Start()
    {
        m_data = CharacterDataHolder.Instance.CurrentCharacter;

        if (m_data != null)
        {
            LoadCharacterData(m_data);
        }
        else
        {
            m_data = new Character
            {
                characterName = m_name.text
            };
        }
    }

    private void LoadCharacterData(Character data)
    {
        m_name.text = data.characterName;

        if (data.pathToAvatarImage != "")
            LoadImage(m_avatarImage, data.pathToAvatarImage);

        if (data.pathToFullBodyImage != "")
            LoadImage(m_fullBodyImage, data.pathToFullBodyImage);

        m_panelController.SetData(data);
    }

    private void OnEnable()
    {
        m_panelController.onLoadFullBodyImage += PastFullBodyImage;
        m_panelController.onLoadAvatarImage += PastAvatarImage;
        m_panelController.onToggleGenerate += HandleToggleButtons;
        m_panelController.onError += ShowErrorPanel;
    }


    public void onExit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void onSaveCharacter()
    {
        if (m_data == null)
        {
            m_data = new Character
            {
                characterName = m_name.text
            };
        }


        m_panelController.FillData(m_data);

        if (m_fullBodyImage.texture != null)
        {
            m_data.pathToFullBodyImage = m_saveLoader.GetCharacterFullBodyFilePath(m_data.characterName);
            m_saveSystemData.SaveImage(m_fullBodyImage.texture as Texture2D, m_data.pathToFullBodyImage);
        }

        if (m_avatarImage.texture != null)
        {
            m_data.pathToAvatarImage = m_saveLoader.GetCharacterAvatarFilePath(m_data.characterName);
            m_saveSystemData.SaveImage(m_avatarImage.texture as Texture2D, m_data.pathToAvatarImage);
        }

        m_saveSystemData.SaveData<Character>(m_data, m_saveLoader.GetCharacterFilePath(m_data.characterName));

        m_card.GetComponent<CardController>().Fill(m_data, m_fullBodyImage.texture as Texture2D, m_avatarImage.texture as Texture2D, m_panelController.GetClass(), m_panelController.GetRace(), m_panelController.GetSex());
        m_saveSystemData.SaveImage(CapturePrefabToImage(), m_saveLoader.GetCharacterCardFilePath(m_data.characterName));

    }

    private void PastFullBodyImage(Texture2D mapImage)
    {
        m_fullBodyImage.texture = mapImage;
    }


    private void PastAvatarImage(Texture2D mapImage)
    {
        m_avatarImage.texture = mapImage;
    }

    private void HandleToggleButtons(bool isGenerating)
    {
        if (isGenerating)
        {
            m_fullBodyImage.texture = null;
            m_avatarImage.texture = null;
        }
        m_exit.interactable = !isGenerating;
        m_save.interactable = !isGenerating;
    }

    private void ShowErrorPanel()
    {
        m_errorPanel.SetActive(true);
    }

    void LoadImage(RawImage img, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }
        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(fileData))
            {
                img.texture = texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + filePath);
                UnityEngine.Object.Destroy(texture);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading texture: " + e.Message);
        }
    }

    private Texture2D CapturePrefabToImage()
    {
        if (m_card == null)
        {
            Debug.LogError("Prefab is not assigned!");
            return null;
        }

        GameObject prefabInstance = Instantiate(m_card);
        SetLayerRecursively(prefabInstance, LayerMask.NameToLayer(targetLayerName));

        Camera renderCamera = SetupRenderCamera();
        renderCamera.cullingMask = 1 << LayerMask.NameToLayer(targetLayerName);

        Texture2D capturedImage = RenderPrefabToTexture(renderCamera);

        DestroyImmediate(prefabInstance);
        DestroyImmediate(renderCamera.gameObject);

        return capturedImage;
    }

    private Camera SetupRenderCamera()
    {
        GameObject cameraObj = new GameObject("PrefabCaptureCamera");
        Camera renderCamera = cameraObj.AddComponent<Camera>();

        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.orthographic = true;
        renderCamera.backgroundColor = Color.green;


        Bounds prefabBounds = GetPrefabBounds(m_card);
        Bounds bounds = GetPrefabBounds(m_card);
        float objectHeight = bounds.size.y;
        float desiredAspect = 1647f / 1021f;
        renderCamera.orthographicSize = objectHeight * 0.5f * desiredAspect;

        renderCamera.transform.position = bounds.center + new Vector3(0, 0, -10);

        return renderCamera;
    }

    private Texture2D RenderPrefabToTexture(Camera renderCamera)
    {
        RenderTexture renderTexture = new RenderTexture(1647, 1021, 24, RenderTextureFormat.ARGB32);
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(1647, 1021, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, 1647, 1021), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        renderCamera.targetTexture = null;
        DestroyImmediate(renderTexture);

        return texture;
    }

    private Bounds GetPrefabBounds(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(prefab.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    private float CalculateOrthoSize(GameObject prefab)
    {
        Bounds bounds = GetPrefabBounds(prefab);
        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f;
        float screenRatio = (float)1647 / 1021;
        float orthoSize = objectSize * screenRatio;
        return orthoSize;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
