using System.Threading.Tasks;
using UnityEngine;

public interface AIManagerInterface
{
    Task<Texture2D> GenerateImageAsync(string prompt);
    Task<string> GenerateTextAsync(string prompt);
}