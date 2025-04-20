using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class PromptBuilder
{
    protected StringBuilder m_prompt = new StringBuilder();
    protected string m_negativePrompt = "blurry, low quality, distorted";

    public abstract string Build();

    public PromptBuilder AddNegativePrompt(string negative)
    {
        m_negativePrompt += ", " + negative;
        return this;
    }

    public PromptBuilder Reset()
    {
        m_prompt.Clear();
        m_negativePrompt = "blurry, low quality, distorted";
        return this;
    }

    public string GetCurrentPrompt() => m_prompt.ToString();
    public string GetNegativePrompt() => m_negativePrompt;
}
