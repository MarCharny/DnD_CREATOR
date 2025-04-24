using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] private RawImage m_fullBodyImage;
    [SerializeField] private RawImage m_avatarImage;
    [SerializeField] private TMP_Text m_name;
    [SerializeField] private TMP_Text m_age;
    [SerializeField] private TMP_Text m_history;
    [SerializeField] private TMP_Text m_stat;

    public void Fill(Character data, Texture2D full, Texture2D ava, string class_, string race, string sex)
    {
        m_fullBodyImage.texture = full;
        m_avatarImage.texture = ava;
        m_name.text = data.characterName;
        m_history.text = data.history;
        m_stat.text = "Lvl: " + data.level.ToString() + " Level \n Class: " + class_ + " \n Race: " + race;
        m_age.text = sex + ", " + data.age.ToString() + " y.o";
    }
}
