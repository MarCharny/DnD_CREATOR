using System;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SocialPlatforms;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class AIReplicateManager : AIManagerInterface
{
    private const string StableDiffusionModel = "stability-ai/stable-diffusion-3.5-medium";
    private const string MistralModel = "mistralai/mistral-7b-v0.1";

    public AIReplicateManager()
    {
        //m_apiKey = GetEnviromentAPIKey();
        //
        //if (string.IsNullOrEmpty(m_apiKey))
        //{
        //    Debug.LogError("Replicate API key not found in environment variables");
        //}
    }

    public async Task<Texture2D> GenerateImageAsync(string prompt, ImageType type)
    {
        if (string.IsNullOrEmpty(m_apiKey))
            return null;

        try
        {
            string ratio = type switch
            {
                ImageType.Fullbody => "9:16",
                ImageType.Avatar => "1:1",
                ImageType.Map => "16:9"
            };
            string inputJson = GenerateImageInputJson(prompt, ratio);
            string url = $"https://api.replicate.com/v1/models/{StableDiffusionModel}/predictions";
            string predictionUrl = await CreatePrediction(url, inputJson);

            string resultUrl = await WaitForPredictionResult(predictionUrl);
            return await DownloadImage(resultUrl);
        }
        catch (Exception e)
        {
            Debug.LogError($"Image generation failed: {e.Message}");
            return null;
        }
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        if (string.IsNullOrEmpty(m_apiKey)) return string.Empty;

        try
        {
            string url = $"https://api.replicate.com/v1/models/{MistralModel}/predictions";
            string inputJson = GenerateInputJson(prompt);
            string predictionUrl = await CreatePrediction(url, inputJson);

            return await WaitForPredictionResult(predictionUrl) ?? string.Empty;
        }
        catch (Exception e)
        {
            Debug.LogError($"Text generation failed: {e.Message}");
            return string.Empty;
        }
    }

    private string GetEnviromentAPIKey()
    {
        string apiKey = Environment.GetEnvironmentVariable("REPLICATE_API_KEY");
        return apiKey;
    }

    private string GenerateInputJson(string prompt)
    {
        return $@"{{
            ""top_k"": 0,
            ""top_p"": 0.95,
            ""prompt"": ""{EscapeJsonString(prompt)}"",
            ""temperature"": 0.3,
            ""length_penalty"": 1,
            ""max_new_tokens"": 170,
            ""prompt_template"": ""{{prompt}}"",
            ""presence_penalty"": 0,
            ""log_performance_metrics"": false
        }}";
    }

    private string GenerateImageInputJson(string prompt, string ratio)
    {
        return $@"{{
            ""prompt"": ""{EscapeJsonString(prompt)}"",
            ""cfg"": 5,
            ""steps"": 30,
            ""aspect_ratio"": ""{EscapeJsonString(ratio)}"",
            ""output_format"": ""png"",
            ""output_quality"": 90,
            ""prompt_strength"": 0.95
        }}";
    }


    private async Task<string> CreatePrediction(string url, string inputJson)
    {
        string requestBody = $"{{\"input\": {inputJson}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {m_apiKey}");
            request.SetRequestHeader("Prefer", "wait");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Full error response: {request.downloadHandler.text}");
                throw new Exception($"Request failed: {request.error}");
            }

            var response = JsonUtility.FromJson<PredictionResponse>(request.downloadHandler.text);
            return response.urls.get;
        }
    }

    private async Task<string> WaitForPredictionResult(string predictionUrl)
    {
        bool isCompleted = false;
        string result = null;
        int maxAttempts = 100;
        int currentAttempt = 0;

        while (!isCompleted && currentAttempt < maxAttempts)
        {
            currentAttempt++;

            using (UnityWebRequest request = UnityWebRequest.Get(predictionUrl))
            {
                request.SetRequestHeader("Authorization", $"Bearer {m_apiKey}");
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Request failed: {request.error}");
                }

                var statusResponse = JsonUtility.FromJson<StatusResponse>(request.downloadHandler.text);

                if (statusResponse.status == "succeeded")
                {
                    if (statusResponse.output != null && statusResponse.output.Length > 0)
                    {
                        result = string.Join("", statusResponse.output);
                    }
                    else if (!string.IsNullOrEmpty(statusResponse.completion))
                    {
                        result = statusResponse.completion;
                    }
                    isCompleted = true;
                }
                else if (statusResponse.status == "failed")
                {
                    throw new Exception($"Prediction failed: {statusResponse.error}");
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        if (!isCompleted)
        {
            throw new Exception("Prediction timed out");
        }

        return result;
    }

    private async Task<Texture2D> DownloadImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return null;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Image download failed: {request.error}");
            }

            return DownloadHandlerTexture.GetContent(request);
        }
    }

    private string EscapeJsonString(string input)
    {
        return input.Replace("\"", "\\\"")
                   .Replace("\n", "\\n")
                   .Replace("\r", "\\r")
                   .Replace("\t", "\\t");
    }


    //Prediction parse help
    [Serializable]
    private class PredictionResponse
    {
        public string id;
        public string version;
        public Urls urls;
    }

    [Serializable]
    private class Urls
    {
        public string get;
    }

    [Serializable]
    private class StatusResponse
    {
        public string status;
        public string[] output;
        public string completion;
        public string error;
    }
}