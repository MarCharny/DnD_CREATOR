using UnityEngine;

public class CharacterDataHolder : MonoBehaviour
{
    public static CharacterDataHolder Instance;

    public Character CurrentCharacter { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}