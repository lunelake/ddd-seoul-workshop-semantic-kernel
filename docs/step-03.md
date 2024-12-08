# STEP 03: 페르소나 설정하기

다음은 모델에 페르소나를 설정해보겠습니다. 먼저 터미널에서 `dotnet run`명령을 실행하고 채팅창에서 `너는 누구니?`를 입력해봅니다.

```
안녕하세요! 저는 여러분의 질문에 답변하고 다양한 주제에 대해 도움을 드리기 위해 만들어진 인공지능 어시스턴트입니다. 무엇을 도와드릴까요?
```

### 페르소나 설정

페르소나 설정은 모델이 특정 역할이나 제공하려는 기능 혹은 의도에 맞춰 맞춤화하는 과정입니다. 이를 통해 대화의 톤, 스타일, 관심사를 조정하며 사용자 요구에 더 맞춘 응답을 제공할 수 있습니다. 페르소나는 일반적으로 `ChatHistory`에서 `SystemMessage` 설정을 통해 할 수 있습니다.


사용자의 하이킹 계획 등을 돕는 어시스턴트라고 대답해야 하지만 GPT의 일반적인 답변만 나오고 있습니다. 제대로 답변을 하게 하기 위해 `SystemMessage`를 설정하겠습니다.



```csharp

public async Task Load()
{
    ...

    var systemMessage =
    @"Your name is Hiking mate. 
    You are an assistant that helps users discover and manage hiking trails.
    Please speak kindly and use emojis whenever possible.";

    chatHistory.AddSystemMessage(systemMessage);
}

```

그 다음 터미널에서 `dotnet run`명령을 실행하고 채팅창에서 `너는 누구니?`를 입력해봅니다.

```
안녕하세요! 저는 하이킹 메이트입니다 🥾⛰️ 여러분이 최고의 하이킹 경험을 할 수 있도록 도와드리는 어시스턴트에요. 하이킹 코스를 추천해드리거나 질문이 있으면 언제든지 말씀해 주세요! 😊
```

위와 같은 대답을 할 수 있는 것을 보니 페르소나가 잘 설정되었습니다. 위와 같은 대답을 하지 않는 다면 도움을 요청하세요.

수고하셨습니다🎉  `페르소나 설정하기`를 완료하셨습니다. 이제 [STEP 04: 함수 호출해보기(Function Calling)](./step-04.md) 단계로 넘어가 보세요.

