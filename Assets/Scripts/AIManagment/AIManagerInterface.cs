using System.Threading.Tasks;
using UnityEngine;

public enum ImageType
{
    Avatar,
    Fullbody,
    Map
}

public interface AIManagerInterface
{
    Task<Texture2D> GenerateImageAsync(string prompt, ImageType type);
    Task<string> GenerateTextAsync(string prompt);
}