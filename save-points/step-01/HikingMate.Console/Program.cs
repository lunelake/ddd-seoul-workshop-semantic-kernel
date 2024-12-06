using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using System.ClientModel;

var modelId = "gpt-4o-mini";
var uri = "https://models.inference.ai.azure.com";
var githubPAT = "GitHub-PAT";//GitHub PAT 입력

var client = new OpenAIClient(new ApiKeyCredential(githubPAT), new OpenAIClientOptions 
{ 
    Endpoint = new Uri(uri) 
});

var kernel = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(modelId, client)
                    .Build();

var chatService = kernel.Services.GetService<IChatCompletionService>();
var chatHistory = new ChatHistory();

while (true)
{
    Console.Write("User : ");
    var input = Console.ReadLine();
    Console.WriteLine();

    await Input(input);
}

async Task Input(string input)
{
    chatHistory.AddUserMessage(input);

    var result = chatService.GetStreamingChatMessageContentsAsync(chatHistory);
    
    Console.Write("Assistant : ");
    var assistantMsg = string.Empty;
    await foreach (var text in result)
    {
        await Task.Delay(20);
        assistantMsg += text;
        Console.Write(text);
    }
    
    Console.WriteLine();
    Console.WriteLine();
}

