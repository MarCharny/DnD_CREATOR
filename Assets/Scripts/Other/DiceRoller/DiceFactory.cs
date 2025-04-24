using UnityEngine;
using UnityEngine.UI;

public class DiceFactory : MonoBehaviour
{
    [SerializeField] private GameObject d2Prefab;
    [SerializeField] private GameObject d4Prefab;
    [SerializeField] private GameObject d6Prefab;
    [SerializeField] private GameObject d8Prefab;
    [SerializeField] private GameObject d10Prefab;
    [SerializeField] private GameObject d12Prefab;
    [SerializeField] private GameObject d20Prefab;
    [SerializeField] private GameObject d100Prefab;

    public DiceController CreateDice(DiceType type, GridLayoutGroup parent)
    {
        GameObject prefab = type switch
        {
            DiceType.D2 => d2Prefab,
            DiceType.D4 => d4Prefab,
            DiceType.D6 => d6Prefab,
            DiceType.D8 => d8Prefab,
            DiceType.D10 => d10Prefab,
            DiceType.D12 => d12Prefab,
            DiceType.D20 => d20Prefab,
            DiceType.D100 => d100Prefab,
            _ => d20Prefab
        };

        GameObject diceObject = Instantiate(prefab, parent.transform);
        SetupRectTransform(diceObject.GetComponent<RectTransform>());
        return diceObject.GetComponent<DiceController>();
    }

    private void SetupRectTransform(RectTransform rt)
    {
        if (rt == null) return;

        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = Vector2.zero;
    }
}