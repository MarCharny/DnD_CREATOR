using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DiceType
{
    D2 = 2,
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20,
    D100 = 100
}

public class DiceData
{
    public DiceType type { get; set; }
    public int val { get; set; }

    public DiceData(DiceType type, int val)
    {
        this.type = type;
        this.val = val;
    }
}

public class DiceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] public DiceData type; 

    public void SetupDice(DiceData value)
    {
        valueText.text = value.val.ToString();
        transform.rotation = Quaternion.Euler(
            0,
            0,
            Random.Range(0, 360f)
        );
    }
}
