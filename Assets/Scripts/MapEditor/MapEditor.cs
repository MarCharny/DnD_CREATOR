using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    [SerializeField] private MapEditorPanelController m_editor;
    [SerializeField] private Button m_exit;
    [SerializeField] private Button m_save;
    [SerializeField] private GameObject m_errorPanel;

    [SerializeField] private RawImage m_mapImage;
    [SerializeField] private GameObject m_mapPointPrefab;
    [SerializeField] private ConfirmPanel m_confirmPanel;
    [SerializeField] private string m_saveDirectory = "maps";
    [SerializeField] private Button m_back;
    [SerializeField] private RectTransform m_points;
    [SerializeField] private TMP_Text m_name;


    private SaveSystemData m_saveSystemData = new SaveSystemData();
    private SaveSystemLoaderMap m_saveLoader = new SaveSystemLoaderMap();

    private Location m_data_common;
    private Location m_data_current;
    private GameObject m_pendingMapPoint;
    private string m_savePath;
    private Stack<Location> m_navigationStack = new Stack<Location>();

    private void Start()
    {
        m_data_common = MapDataHolder.Instance.CurrentMap;
        if (m_data_common != null)
        {
            m_data_current = m_data_common;
            LoadMapData(m_data_common);
        }
        else
        {
            m_data_common = new Location
            { 
                locationName = m_name.text
            };
            m_data_current = m_data_common;
        }
    }

    private void Awake()
    {
        m_savePath = Path.Combine(Application.persistentDataPath, m_saveDirectory);
        m_back.onClick.AddListener(GoBackToParentMap);

        m_confirmPanel.gameObject.SetActive(false);
        m_confirmPanel.OnConfirm += ConfirmNewLocation;
        m_confirmPanel.OnCancel += CancelNewLocation;
    }

    private void OnEnable()
    {
        m_editor.onMapLoaded += PastMapImage;
        m_editor.ToggleGenerate += HandleToggleButtons;
        m_editor.onError += ShowErrorPanel;
    }

    private void UpdateUI()
    {
        m_name.text = m_data_current.locationName;
        m_back.gameObject.SetActive(m_navigationStack.Count > 0);
    }

    private void LoadMapVisuals()
    {
        if (!string.IsNullOrEmpty(m_data_current.pathToMapImage) && File.Exists(m_data_current.pathToMapImage))
        {
            byte[] imageData = File.ReadAllBytes(m_data_current.pathToMapImage);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            if (texture.LoadImage(imageData))
            {
                m_mapImage.texture = texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file: " + m_data_current.pathToMapImage);
                UnityEngine.Object.Destroy(texture);
            }
        }
        else
        {
            m_mapImage.texture = null;
        }

        ClearMapPoints();

        foreach (Location location in m_data_current.subLocations)
        {
            CreateLocationPoint(location, false);
        }
    }

    public void LoadMapData(Location data)
    {
        m_navigationStack.Push(m_data_current);
        m_name.text = m_data_current.locationName;
        m_data_current = data;
        if (data.locationName == m_data_common.locationName)
        {
            m_navigationStack.Clear();
        }
        UpdateUI();
        LoadMapVisuals();
    }

    private void ClearMapPoints()
    {
        foreach (Transform child in m_points)
        {
            if (child.gameObject != m_pendingMapPoint)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateLocationPoint(Location location, bool isPending)
    {
        GameObject point = Instantiate(m_mapPointPrefab, m_points);

        point.transform.localPosition = Vector3.zero;
        point.transform.localRotation = Quaternion.identity;
        point.transform.localScale = Vector3.one;

        RectTransform rt = point.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = location.positionOnParentMap;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }

        LocationPoint locationPoint = point.GetComponent<LocationPoint>();
        if (locationPoint != null)
        {
            locationPoint.Initialize(location, this);
        }

        if (isPending)
        {
            m_pendingMapPoint = point;
        }
    }

    public void OnMapClick()
    {
        if (m_mapImage.texture == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            m_points,
            Input.mousePosition,
            null,
            out localPoint);

        StartCreatingNewLocation(localPoint);
    }

    private void StartCreatingNewLocation(Vector2 position)
    {
        if (m_pendingMapPoint != null)
        {
            Destroy(m_pendingMapPoint);
        }

        Location tempLocation = new Location
        {
            positionOnParentMap = position
        };

        CreateLocationPoint(tempLocation, true);
        ShowConfirmPanel(position);
    }

    private void ShowConfirmPanel(Vector2 position)
    {
        m_confirmPanel.ClearInput();
        m_confirmPanel.gameObject.SetActive(true);
    }

    private void ConfirmNewLocation(string locationName, Vector2 position)
    {
        if (m_pendingMapPoint == null) return;

        Location newLocation = new Location
        {
            locationName = locationName,
            positionOnParentMap = position,
            subLocations = new List<Location>(),
            parentMapName = m_data_current.locationName
        };
        

        m_data_current.subLocations.Add(newLocation);

        LocationPoint locationPoint = m_pendingMapPoint.GetComponent<LocationPoint>();
        if (locationPoint != null)
        {
            locationPoint.Initialize(newLocation, this);
        }

        SaveMap();

        m_pendingMapPoint = null;
        m_confirmPanel.gameObject.SetActive(false);
    }


    private void CancelNewLocation()
    {
        if (m_pendingMapPoint != null)
        {
            Destroy(m_pendingMapPoint);
            m_pendingMapPoint = null;
        }

        m_confirmPanel.gameObject.SetActive(false);
    }


    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveMap()
    {
        SaveLocationRecursive(m_data_common, "");
    }

    private void SaveLocationRecursive(Location location, string relativePath)
    {
        string folder = Path.Combine(m_savePath, relativePath, location.locationName);
        Directory.CreateDirectory(folder);

        if (m_mapImage.texture is Texture2D tex && m_data_current == location)
        {
            m_saveSystemData.SaveImage(m_mapImage.texture as Texture2D, m_saveLoader.GetMapImagePath(Path.Combine(relativePath, location.locationName)));
            location.pathToMapImage = m_saveLoader.GetMapImagePath(relativePath + location.locationName);
        }

        m_saveSystemData.SaveData(m_data_current, m_saveLoader.GetMapFilePath(Path.Combine(relativePath, location.locationName)));

        foreach (var sub in location.subLocations)
        {
            SaveLocationRecursive(sub, Path.Combine(relativePath, location.locationName));
        }
    }

    public void NavigateToLocation(Location location)
    {
        SaveMap();
        m_navigationStack.Push(m_data_current);
        m_data_current = location;
        LoadMapData(location);
    }

    private void GoBackToParentMap()
    {
        if (m_navigationStack.Count > 0)
        {
            SaveMap();
            m_navigationStack.Pop();
            Location parentMap = m_navigationStack.Pop();
            LoadMapData(parentMap);
        }
        else
        {
            Exit();
        }
    }

    private void PastMapImage(Texture2D mapImage)
    {
        m_mapImage.texture = mapImage;
    }

    private void HandleToggleButtons(bool isGenerating)
    {
        m_exit.interactable = !isGenerating;
        m_save.interactable = !isGenerating;
        m_back.interactable = !isGenerating;
    }

    private void ShowErrorPanel()
    {
        m_errorPanel.SetActive(true);
    }

}

