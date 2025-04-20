using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPromptBuilder : PromptBuilder
{
    public MapPromptBuilder()
    {
        m_prompt.Append("Create a top-down view map of ");
    }

    public MapPromptBuilder AddMapType(string type)
    {
        m_prompt.Append($"{type}, ");
        return this;
    }

    public MapPromptBuilder AddLandscape(string landscape)
    {
        m_prompt.Append($"with {landscape}, ");
        return this;
    }

    public MapPromptBuilder AddStyle(string style)
    {
        m_prompt.Append($"in {style} style, ");
        return this;
    }

    public override string Build()
    {
        if (m_prompt.Length > 0)
        {
            m_prompt.Length -= 2;
        }

        m_prompt.Append(", highly detailed, 4K, digital painting");
        return m_prompt.ToString();
    }
}