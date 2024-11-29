
#region 0.Semantic Kernel Nuget 설치

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using HikingMate;

#endregion

#region 1. Kernel 생성 및 Credential

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(Credential.AzureOpenAIDeploymentName, Credential.AzureOpenAIEndPoint, Credential.AzureOpenAIApiKey);

var kernel = builder.Build();
var chatService = kernel.Services.GetService<IChatCompletionService>();
var chatHistory = new ChatHistory();

#endregion

#region 2. Input

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

    #region 3. Streaming

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

    #endregion
}

#endregion