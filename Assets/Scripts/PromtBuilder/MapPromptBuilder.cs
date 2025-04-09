using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPromptBuilder : PromptBuilder
{
    public MapPromptBuilder()
    {
        prompt.Append("Create a top-down view map of ");
    }

    public MapPromptBuilder AddMapType(string type)
    {
        prompt.Append($"{type}, ");
        return this;
    }

    public MapPromptBuilder AddLandscape(string landscape)
    {
        prompt.Append($"with {landscape}, ");
        return this;
    }

    public MapPromptBuilder AddStyle(string style)
    {
        prompt.Append($"in {style} style, ");
        return this;
    }

    public override string Build()
    {
        if (prompt.Length > 0)
        {
            prompt.Length -= 2;
        }

        prompt.Append(", highly detailed, 4K, digital painting");
        return prompt.ToString();
    }
}