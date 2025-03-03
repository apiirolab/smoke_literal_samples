// See https://aka.ms/new-console-template for more information

using ConsoleAppSample;

Console.WriteLine("Hello, World!");

string apiKey = "your-api-key-here";
var client = new OpenAiClient(apiKey);

try
{
    string response = await client.GetChatCompletionAsync("Tell me a joke.");
    Console.WriteLine("AI Response: " + response);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

