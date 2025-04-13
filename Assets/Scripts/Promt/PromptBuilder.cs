using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class PromptBuilder
{
    protected StringBuilder prompt = new StringBuilder();
    protected string negativePrompt = "blurry, low quality, distorted";

    public abstract string Build();

    public PromptBuilder AddNegativePrompt(string negative)
    {
        negativePrompt += ", " + negative;
        return this;
    }

    public PromptBuilder Reset()
    {
        prompt.Clear();
        negativePrompt = "blurry, low quality, distorted";
        return this;
    }

    public string GetCurrentPrompt() => prompt.ToString();
    public string GetNegativePrompt() => negativePrompt;
}
