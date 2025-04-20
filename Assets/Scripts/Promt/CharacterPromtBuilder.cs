using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterPromptBuilder : PromptBuilder
{
    private bool m_isFullBody = false;

    public CharacterPromptBuilder(bool isFullBody = false)
    {
        this.m_isFullBody = isFullBody;
        m_prompt.Append(isFullBody
            ? "Create a full-body character portrait of "
            : "Create a close-up portrait of ");
    }

    public CharacterPromptBuilder AddCharacterClass(string type)
    {
        m_prompt.Append($"{type}, ");
        return this;
    }

    public CharacterPromptBuilder AddAppearance(string appearance)
    {
        m_prompt.Append($"{appearance}, ");
        return this;
    }

    public CharacterPromptBuilder AddClothing(string clothing)
    {
        m_prompt.Append($"wearing {clothing}, ");
        return this;
    }

    public override string Build()
    {
        if (m_prompt.Length > 0)
            m_prompt.Length -= 2;

        m_prompt.Append(m_isFullBody
            ? ", dynamic pose, full-body, highly detailed, 8K"
            : ", intricate facial details, studio lighting, 4K");

        return m_prompt.ToString();
    }
}