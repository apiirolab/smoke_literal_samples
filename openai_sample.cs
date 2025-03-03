using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OpenAIClient
{
    private readonly string _apiKey;
    private static readonly HttpClient _httpClient = new HttpClient();

    public OpenAIClient(string apiKey)
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

        string jsonContent = JsonConvert.SerializeObject(requestBody);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        requestMessage.Headers.Add("Authorization", $"Bearer {_apiKey}");

        HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            return jsonResponse.choices[0].message.content.ToString();
        }
        else
        {
            throw new Exception($"Error: {response.StatusCode} - {responseContent}");
        }
    }
}

// Example usage:
public class Program
{
    public static async Task Main()
    {
        string apiKey = "your-api-key-here";
        var client = new OpenAIClient(apiKey);

        try
        {
            string response = await client.GetChatCompletionAsync("Tell me a joke.");
            Console.WriteLine("AI Response: " + response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
