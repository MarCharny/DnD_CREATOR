using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AIReplicateManager : AIManagerInterface
{
    public AIReplicateManager()
    {
        m_apiKey = GetEnviromentAPIKey();

        if (string.IsNullOrEmpty(m_apiKey))
        {
            Debug.LogError("Replicate API key not found in environment variables");
        }
    }

    public async Task<Texture2D> GenerateImageAsync(string prompt)
    {
        if (string.IsNullOrEmpty(m_apiKey))
            return null;

        try
        {
            string inputJson = $"{{\"prompt\":\"{EscapeJsonString(prompt)}\", \"width\": 512, \"height\": 512}}";
            string predictionUrl = await CreatePrediction(StableDiffusionModel, inputJson);

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
            string inputJson = $"{{\"prompt\":\"{EscapeJsonString(prompt)}\", \"max_new_tokens\": 100}}";
            string predictionUrl = await CreatePrediction(MistralModel, inputJson);

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

    private async Task<string> CreatePrediction(string model, string inputJson)
    {
        string url = "https://api.replicate.com/v1/predictions";
        string requestBody = $"{{\"version\": \"{model}\", \"input\": {inputJson}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Token {m_apiKey}");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
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
        int maxAttempts = 30;
        int currentAttempt = 0;

        while (!isCompleted && currentAttempt < maxAttempts)
        {
            currentAttempt++;

            using (UnityWebRequest request = UnityWebRequest.Get(predictionUrl))
            {
                request.SetRequestHeader("Authorization", $"Token {m_apiKey}");
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
                        result = statusResponse.output[0];
                    }
                    else if (!string.IsNullOrEmpty(statusResponse.completion))
                    {
                        result = statusResponse.completion;
                    }
                    isCompleted = true;
                }
                else if (statusResponse.status == "failed")
                {
                    throw new Exception("Prediction failed");
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
    }
}