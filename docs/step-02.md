# STEP 02: Blazor로 AI 웹 앱 만들어보기

이 단계에서는 Semantic Kernel을 Blazor에 적용해보고, 기본적인 채팅 경험 구현, Markdown 표시를 Blazor에서 어떻게 하는 지 알아봅니다.

## 모델 응답을 받고 화면에 표시

[Step-02-start](../save-points/step-02-start/) 폴더의 `HikingMateWebApp`을 복사해서 다른 곳에 옮기거나, 해당 소스 코드로 바로 연습을 해보겠습니다.

해당 프로젝트가 있는 경로에서 터미널 실행 혹은 cd로 이동 후 `HikingMate.WebApp\HikingMate.WebApp`로 이동한 후 `dotnet run`을 실행합니다.

```
cd HikingMate.WebApp\HikingMate.WebApp
dotnet run
```

그러면 웹 프로젝트가 실행될 것이고, 기본적인 채팅 인터페이스가 보일 것입니다. 만약 브라우저에서 웹페이지가 보이지 않는다면 브라우저에서 [http://localhost:5047](http://localhost:5047)에 들어가 봅니다. 여기에서 채팅창에 입력을 누르고 엔터를 눌러도 아무것도 실행되지 않습니다. 그렇다면 먼저 모델을 연결하고 사용자 입력에 따라 모델이 응답이 표시되도록 해보겠습니다.

먼저 Kernel 개체를 생성하겠습니다. `HikingMate.WebApp/Components/Pages/ChatRoom.razor` 파일을 엽니다. 그리고 아래 `@code` 영역에 코드를 추가합니다.
`Kernel`, `ChatCompletionService`, `ChatHistory`를 선언해줍니다.

```csharp
Kernel? kernel;
ChatHistory? chatHistory;
IChatCompletionService chatCompletionService;
```

 `Load` 메서드를 추가합니다. `Load` 메서드에서는 `Kernel`, `ChatCompletionService`, `ChatHistory` 개체를 생성합니다. 모델은 `gpt-4o`를 사용하겠습니다.

```csharp
public async Task Load()
{
    await Task.Delay(1);
    messages.Clear();
    var modelId = "gpt-4o";
    var client = GitHubModelsClient.GetClient();
    kernel = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(modelId, client)
                        .Build();
    chatCompletionService = kernel.Services.GetService<IChatCompletionService>();
    chatHistory = new ChatHistory();
}
```

그리고 Load 메서드를 소스 코드 내의 `OnInitializedAsync` 에서 실행하겠습니다.

```csharp
protected async override Task OnInitializedAsync()
{
    await Load();
}
```

그리고 `Services/GitHubClient.cs`파일로 이동해서 이전에 사용했던 PAT 토큰을 입력해줍니다.

```csharp
public static OpenAIClient GetClient()
{
    var githubPAT = "github-PAT";//PAT 토큰으로 교체
    var uri = "https://models.inference.ai.azure.com";
    var client = new OpenAIClient(new ApiKeyCredential(githubPAT), new OpenAIClientOptions
    {
        Endpoint = new Uri(uri)
    });

    return client;
}
```

자, 이제 모델 사용 준비를 마쳤습니다. 이제 사용자 입력을 받아 모델의 응답을 받을 수 있도록 UI 작업을 해보겠습니다. 다시 `HikingMate.WebApp/Components/Pages/ChatRoom.razor` 파일을 엽니다.

먼저 input을 선언합니다.

```csharp
string? input;
```

파일의 html 영역에서 textarea를 찾아 `@bind="@input"`와 `@bind:event="oninput"`를 입력합니다. 이는 텍스트 입력 컨트롤에서 oninput 이벤트가 발생할 때 텍스트 입력 컨트롤의 값을 방금 선언한 `input`과 바인딩시킵니다. 

```html
<textarea class="input-lg" placeholder="무엇을 도와드릴까요?" @bind="@input" @bind:event="oninput"></textarea>
```

그리고 `@code` 영역에 input을 `ChatHistory`에 추가하고 모델에 보내고 응답을 받는 코드를 추가하겠습니다.

```csharp
private async Task Send()
{
    if (string.IsNullOrEmpty(input) || chatHistory == null || kernel == null)
        return;

    var userMessage = new ChatMessage(ChatMessageRole.User, input);
    messages.Add(userMessage);
    chatHistory.AddUserMessage(input);
    input = string.Empty;
    await InvokeAsync(StateHasChanged);

    var assistantMessage = new ChatMessage(ChatMessageRole.Assistant);
    messages.Add(assistantMessage);

    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory);
    await foreach (var text in result)
    {
        await Task.Delay(20);
        assistantMessage.Message += text;
        await InvokeAsync(StateHasChanged);
    }
}

```

그리고 Button이 클릭될 때 `Send`메서드를 실행하도록 html 영역에서 Send 버튼 코드에 `@onclick="@(Send)"`를 추가해 메서드를 연결합니다.

```html
<button class="btn send-button" @onclick="@(Send)">
    <i class="fas fa-paper-plane"></i>
</button>
```

그리고 `dotnet run`을 터미널에 입력해 앱을 실행해보겠습니다. 그리고 `안녕`이라고 입력하고 버튼을 눌러 모델이 정상적으로 응답하는 지 확인합니다. 만약 모델의 응답이 제대로 화면에 표시되지 않는 경우 도움을 요청합니다.

### 엔터키를 눌렀을 때 채팅이 보내지도록 하기

지금까지 구현된 것으로는 보내기 버튼을 눌러야 채팅이 보내집니다. 하지만 채팅을 치면서 보내기 버튼을 클릭하는 것은 불편합니다. 엔터를 치면 채팅이 보내지도록 하겠습니다. 먼저 `HandleKeyDown` 메서드를 추가해 키보드 이벤트에서 엔터키가 입력되면 `Send` 메서드를 실행하도록 합니다.

```csharp
private async Task HandleKeyDown(KeyboardEventArgs e)
{
    var isEnterKey = e.Key == "Enter" && !e.ShiftKey;
    if (isEnterKey)
        await Send();
}
```

그 다음 html 영역의 textarea에 `@onkeydown="HandleKeyDown"` 코드를 추가해 `HandleKeydown` 메서드를 연결합니다.

```html
<textarea class="input-lg" @onkeydown="HandleKeyDown" placeholder="무엇을 도와드릴까요?" @bind="@input" @bind:event="oninput"></textarea>
```

그리고 `dotnet run`을 터미널에 입력해 앱을 실행해보겠습니다. 그리고 `안녕`이라고 입력하고 엔터를 눌러 모델이 정상적으로 응답하는 지 확인합니다.

수고하셨습니다🎉  `Blazor로 AI 웹 앱 만들어보기`를 완료하셨습니다. 이제 [STEP 03: 페르소나 설정하기](./step-03.md) 단계로 넘어가 보세요.






