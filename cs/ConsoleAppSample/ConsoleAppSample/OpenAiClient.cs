namespace ConsoleAppSample;

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class OpenAiClient
{
    private readonly string _apiKey;
    private static readonly HttpClient _httpClient = new HttpClient();

    public OpenAiClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<string> GetChatCompletionAsync(string prompt, string model = "gpt-3.5-turbo")
    {
        string url = "https://api.openai.com/v1/chat/completions";

        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You are an AI assistant." },
                new { role = "user", content = prompt }
            },
            max_tokens = 150
        };

        string jsonContent =requestBody.ToString();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");

        HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return responseContent;
        }
        
        throw new Exception($"Error: {response.StatusCode} - {responseContent}");
        
    }
}

