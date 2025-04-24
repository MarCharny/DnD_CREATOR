using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class PromptBuilder
{
    protected StringBuilder m_prompt = new StringBuilder();
    protected string m_negativePrompt = "not blurry, not low quality, not distorted";

    public abstract string Build();

    public PromptBuilder Reset()
    {
        m_prompt.Clear();
        return this;
    }

    public string GetCurrentPrompt() => m_prompt.ToString();
    public string GetNegativePrompt() => m_negativePrompt;
}
