using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPromptBuilder : PromptBuilder
{
    public MapPromptBuilder AddBasic()
    {
        m_prompt.Append("Create a top-down view map of ");
        return this;
    }

    public MapPromptBuilder AddCommon(string desc)
    {
        m_prompt.Append($"a {desc} ");
        return this;
    }

    public MapPromptBuilder AddStyle(string desc)
    {
        m_prompt.Append($"in {desc}, style ");
        return this;
    }

    public MapPromptBuilder AddObjects(string desc)
    {
        m_prompt.Append($"with {desc} on it, ");
        return this;
    }

    public override string Build()
    {
        if (m_prompt.Length > 0)
        {
            m_prompt.Length -= 2;
        }

        m_prompt.Append(", highly detailed, 4K, digital painting");
        m_prompt.Append($", {m_negativePrompt}");
        return m_prompt.ToString();
    }
}