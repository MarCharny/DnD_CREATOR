using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollerController : MonoBehaviour
{
    [SerializeField] private DiceRollerView view;

    private void OnEnable()
    {
        view.OnRoll += HandleRollRequest;
    }

    private void OnDisable()
    {
        view.OnRoll -= HandleRollRequest;
    }

    private void HandleRollRequest(DiceType type, int count)
    {
        view.ClearDiceContainer();
        List<DiceData> diceResults = RollDice(type, count);

        foreach (DiceData diceData in diceResults)
        {
            view.CreateDiceView(diceData);
        }
    }

    private List<DiceData> RollDice(DiceType type, int count)
    {
        List<DiceData> results = new List<DiceData>();

        for (int i = 0; i < count; i++)
        {
            int maxValue = (int)type;
            int value = Random.Range(1, maxValue + 1);
            results.Add(new DiceData(type, value));
        }

        return results;
    }

}
