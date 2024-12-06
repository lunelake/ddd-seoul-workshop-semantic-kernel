# STEP 01: Semantic Kernel 사용해보기

이 단계에서는 Semantic Kernel에서 콘솔 애플리케이션을 만들고 앞서 생성한 모델을 연결하여 GPT의 응답을 받아봅니다.

## GitHub Models에서 모델 사용

워크샵을 진행하기 위해서는 GitHub Models에서 만들어진 GPT 모델을 사용할 수 있어야 합니다. 만약 모델이 만들어져있지 않다면 [Step00#github-models를-이용해-다양한-모델을-생성하고-연결](./step-00.md#github-models를-이용해-다양한-모델을-생성하고-연결)을 참고해 `PAT`를 얻은 뒤 진행해야 합니다.


## Console 프로젝트 생성

터미널을 열어 아래의 명령어를 입력해 콘솔 프로젝트를 생성합니다.

```
dotnet new console -n HikingMate.Console
```

`HikingMate.Console` 폴더로 이동합니다.
```
cd HikingMate.Console
```

### Semantic Kernel Nuget 패키지 설지

아래의 명령어를 터미널에 입력해 Semantic Kernel 패키지를 설치합니다.

```
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.Extensions.DependencyInjection
```

## Console 프로젝트에서 LLM 모델 테스트

방금 만든 Console 프로젝트에서 GitHub Models으로 `GPT-4o-mini` 모델을 이용해 질의를 넣고 응답을 받아보기 위한 간단한 코딩을 해보겠습니다.

### 네임스페이스 생성

우선 VS Code에서 `HikingMate.Console` 폴더로 이동하고 `Program.cs` 파일을 클릭합니다.
그리고 네임스페이스를 입력해줍니다.

```
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using System.ClientModel;
```

### Kernel 생성

그리고 Kernel을 빌드합니다. 아래 코드에 모델 명이 `gpt-4o-mini`로 입력되어 있고, uri 엔드포인트도 설정이 되어 있습니다. 아래 코드에 `githubPAT`를 설정해주어야 하는데, 이전에 GitHub Models에서 생성했던 `PAT 토큰`을 넣어주면 됩니다. 참고로 Semantic kernel은 GitHub Models 뿐만 아니라 Open AI, Azure OpenAI에서 제공된 모델과도 당연히 연동됩니다.

```
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
```

### ChatService 생성

위에 입력한 코드 아래에 아래 코드를 붙여 넣어줍니다. `IChatCompletionService`는 Semantic Kernel의 인터페이스로, `Chat Completion` 기능을 제공하고 ChatHistory 클래스는 대화 이력을 관리하는데, 이는 GPT 모델이 대화 컨텍스트를 바탕으로 대답할 수 있게 합니다.

```
var chatService = kernel.Services.GetService<IChatCompletionService>();
var chatHistory = new ChatHistory();
```

### 사용자 입력 처리

콘솔에서 사용자 입력을 받아 처리해봅니다. 아래의 코드는 콘솔에서 사용자 입력을 받고 콘솔에 출력시킴과 동시에 ChatHistory에 대화를 추가시킵니다.

```
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
}
```

### Streaming 처리

`ChatGPT`를 사용해보신 분들을 아시겠지만 ChatGPT에서 응답은 한 번에 오지 않고, 한마디씩 끊어서 옵니다. 이를 처리해보겠습니다.

```
async Task Input(string input)
{
    chatHistory.AddUserMessage(input);

    //코드 추가 위치
}
```
바로 전에 입력한 코드의 Input 메서드 안에 아래의 코드를 추가합니다.

```
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

```

터미널에서 `dotnet run`을 입력해 콘솔 애플리케이션을 실행합니다. 

```
dotnet run
```

`안녕`이라고 입력해봅니다. 답변이 오는 지 확인하고 답변이 제대로 온다면 다른 문장도 입력해봅니다.

🎉 축하합니다! `Semantic Kernel 사용해보기`가 완료되었습니다. 이제 [STEP 02: Blazor로 AI 웹 앱 만들어보기](./step-02.md) 단계로 넘어가 보세요.




