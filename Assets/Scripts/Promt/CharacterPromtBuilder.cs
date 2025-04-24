using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterPromptBuilder : PromptBuilder
{
    private bool m_isFullBody = false;

    public CharacterPromptBuilder AddBasic(bool isFullBody = false)
    {
        this.m_isFullBody = isFullBody;
        m_prompt.Append(isFullBody
            ? "Create a full-body character picture of "
            : "Create a close-up face portrait of ");

        return this;
    }

    public CharacterPromptBuilder AddRace(string desc)
    {
        m_prompt.Append($" {desc} being, ");
        return this;
    }

    public CharacterPromptBuilder AddSex(int isMale)
    {
        m_prompt.Append(isMale == 0 ? " man " : "woman ");
        return this;
    }

    public CharacterPromptBuilder AddAge(string desc)
    {
        m_prompt.Append($"of {desc} years old, ");
        return this;
    }

    public CharacterPromptBuilder AddAppearance(string desc)
    {
        m_prompt.Append($" with {desc}, ");
        return this;
    }

    public CharacterPromptBuilder AddClothing(string desc)
    {
        m_prompt.Append($"wearing {desc}, ");
        return this;
    }

    public override string Build()
    {
        if (m_prompt.Length > 0)
            m_prompt.Length -= 2;

        m_prompt.Append(m_isFullBody
            ? ", dynamic pose, full-body, highly detailed, 8K"
            : ", intricate facial details, studio lighting, 4K");

        m_prompt.Append(m_negativePrompt);

        return m_prompt.ToString();
    }

}