using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterPromptBuilder : PromptBuilder
{
    private bool isFullBody = false;

    public CharacterPromptBuilder(bool isFullBody = false)
    {
        this.isFullBody = isFullBody;
        prompt.Append(isFullBody
            ? "Create a full-body character portrait of "
            : "Create a close-up portrait of ");
    }

    public CharacterPromptBuilder AddCharacterClass(string type)
    {
        prompt.Append($"{type}, ");
        return this;
    }

    public CharacterPromptBuilder AddAppearance(string appearance)
    {
        prompt.Append($"{appearance}, ");
        return this;
    }

    public CharacterPromptBuilder AddClothing(string clothing)
    {
        prompt.Append($"wearing {clothing}, ");
        return this;
    }

    public override string Build()
    {
        if (prompt.Length > 0)
            prompt.Length -= 2;

        prompt.Append(isFullBody
            ? ", dynamic pose, full-body, highly detailed, 8K"
            : ", intricate facial details, studio lighting, 4K");

        return prompt.ToString();
    }
}