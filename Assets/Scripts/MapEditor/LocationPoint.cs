using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationPoint : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private Location location;
    private MapEditor mapEditor;

    public void Initialize(Location location, MapEditor editor)
    {
        this.location = location;
        this.mapEditor = editor;

        nameText.text = location.locationName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnPointClicked);
    }

    private void OnPointClicked()
    {
        mapEditor.NavigateToLocation(location);
    }
}