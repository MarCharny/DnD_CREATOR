using System;
using UnityEngine.UI;
using UnityEngine;

public class DiceRollerView : MonoBehaviour
{
    [SerializeField] private ToggleGroup diceTypeToggleGroup;
    [SerializeField] private ToggleGroup diceCountToggleGroup;
    [SerializeField] private Button rollButton;
    [SerializeField] private GridLayoutGroup diceContainer;
    [SerializeField] private DiceFactory diceFactory;

    public event Action<DiceType, int> OnRoll;

    private void Awake()
    {
        rollButton.onClick.AddListener(HandleRollButtonClick);
        ValidateGridLayout(); 
    }

    private void ValidateGridLayout()
    {
        if (diceContainer == null)
        {
            Debug.LogError("Dice Container with GridLayoutGroup is not assigned!");
            return;
        }

        diceContainer.childAlignment = TextAnchor.MiddleCenter;
        diceContainer.cellSize = new Vector2(90, 90);
    }

    private void HandleRollButtonClick()
    {
        DiceType selectedType = GetSelectedDiceType();
        int selectedCount = GetSelectedDiceCount();
        OnRoll?.Invoke(selectedType, selectedCount);
    }

    private DiceType GetSelectedDiceType()
    {
        foreach (Toggle toggle in diceTypeToggleGroup.ActiveToggles())
        {
            ToggleCtrl toggleCtrl = toggle.GetComponent<ToggleCtrl>();
            if (toggleCtrl)
            {
                return toggleCtrl.m_value switch
                {
                    2 => DiceType.D2,
                    4 => DiceType.D4,
                    6 => DiceType.D6,
                    8 => DiceType.D8,
                    10 => DiceType.D10,
                    12 => DiceType.D12,
                    20 => DiceType.D20,
                    100 => DiceType.D100,
                    _ => DiceType.D20
                };
            }
        }
        return DiceType.D20;
    }

    private int GetSelectedDiceCount()
    {
        foreach (Toggle toggle in diceCountToggleGroup.ActiveToggles())
        {
            ToggleCtrl toggleCtrl = toggle.GetComponent<ToggleCtrl>();
            if (toggleCtrl)
            {
                return toggleCtrl.m_value;
            }
        }
        return 1;
    }

    public void ClearDiceContainer()
    {
        diceContainer.enabled = false; 
        foreach (Transform child in diceContainer.transform)
        {
            Destroy(child.gameObject);
        }
        diceContainer.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(diceContainer.GetComponent<RectTransform>());

    }

    public DiceController CreateDiceView(DiceData data)
    {
        DiceController dice = diceFactory.CreateDice(data.type, diceContainer);
        dice.SetupDice(data);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(diceContainer.GetComponent<RectTransform>());
        return dice;
    }
}