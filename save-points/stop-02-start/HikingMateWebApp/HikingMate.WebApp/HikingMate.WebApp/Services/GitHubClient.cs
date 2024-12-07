using OpenAI;
using System.ClientModel;
using static System.Net.WebRequestMethods;

public class GitHubModelsClient
{
    public static OpenAIClient GetClient()
    {
        var githubPAT = "github-PAT";
        var uri = "https://models.inference.ai.azure.com";
        var client = new OpenAIClient(new ApiKeyCredential(githubPAT), new OpenAIClientOptions
        {
            Endpoint = new Uri(uri)
        });

        return client;
    }
}