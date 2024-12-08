# 함수 호출해보기(Function Calling)

HikingMate 서비스에는 위시리스트 기능이 있습니다. 가고 싶은 트래킹 코스를 저장해 볼 수 있는 기능입니다. 이 기능에 대한 코드를 추가하고 LLM 모델과 연결해서 기능을 대화로 작동시켜보겠습니다.

먼저 `Services` 폴더에 `HikingMateWishlistService.cs` 파일을 추가 하고, 아래의 코드를 복사하겠습니다.
아래의 코드는 단순한 CRUD를 구현한 코드로, 위시리스트에 등록한 하이킹 트레일들을 추가, 수정, 삭제, 열람하는 기능이 간단히 구현된 코드입니다.

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

public class HikingMateWishlistService
{
    public event EventHandler? NeedUpdateUI;
    public List<HikingTrail> wishlistItems;

    public HikingMateWishlistService(List<HikingTrail> wishlistItems)
    {
        this.wishlistItems = wishlistItems;
    }

    public List<HikingTrail> AddMultipleHikingTrailsToWishlist(List<HikingTrail> items)
    {
        foreach (var item in items)
            item.Id = Guid.NewGuid().ToString();

        wishlistItems.AddRange(items);
      
        NeedUpdateUI?.Invoke(this, new EventArgs());

        return wishlistItems;
    }

    public HikingTrail AddHikingTrailsToWishlist(HikingTrail item)
    {
        item.Id = Guid.NewGuid().ToString();

        wishlistItems.Add(item);
        NeedUpdateUI?.Invoke(this, new EventArgs());

        return item;
    }

    public (HikingTrail? Result, string Message) UpdateHikingTrailInWishlist(HikingTrail hikingTrail)
    {
        var wishlist = wishlistItems.FirstOrDefault(x => x.Id == hikingTrail.Id);
        if (wishlist == null)
            return (null, "목록에 해당 Wishlist가 없습니다.");

        wishlistItems.Remove(wishlist);
        wishlistItems.Add(hikingTrail);

        NeedUpdateUI?.Invoke(this, new EventArgs());

        return (hikingTrail, "성공");
    }

    public (bool Success, string Message) RemoveHikingTrailFromWishlist(string id)
    {
        var wishlist = wishlistItems.FirstOrDefault(x => x.Id == id);
        if (wishlist == null)
            return (false, "목록에 해당 Wishlist가 없습니다.");

        wishlistItems.Remove(wishlist);

        NeedUpdateUI?.Invoke(this, new EventArgs());
        return (true, "성공");
    }
}

```

위 코드는 우리가 개발 할 때 흔하게 보는 코드 형태입니다. `Semantic Kernel`에 이 코드를 연결해서 LLM이 이 코드를 실행하게 할 수 있는데요, 어떻게 할 수 있는 지 살표보겠습니다.

일단 Semantic Kernel에 사용할 수 있는 메서드 위에 `[KernelFunction]`이라는 Attribute를 달고 LLM이 이 메서드가 어떠한 역할을 하는 지 이해할 수 있도록 `[Description("메서드 설명")]` 설명을 달아줍니다. `gpt-4o` 기준으로 프롬프트 메시지 등은 한글도 잘 이해하지만, 아직 함수 호출의 경우 영어로 하는 경우 잘 이해하는 경우가 많습니다.

그래서 위에 추가한 코드에 영어로 Attribute와 설명을 달아주겠습니다.

```csharp

[KernelFunction]
[Description("Adds multiple hiking trails to the user's wishlist in a single request.")]
public List<HikingTrail> AddMultipleHikingTrailsToWishlist(List<HikingTrail> items)
...

[KernelFunction]
[Description("Adds new hiking trails to the user's wishlist.")]
public HikingTrail AddHikingTrailsToWishlist(HikingTrail item)
...

[KernelFunction]
[Description("Adds new hiking trails to the user's wishlist.")]
public (HikingTrail? Result, string Message) UpdateHikingTrailInWishlist(HikingTrail hikingTrail)
...

[KernelFunction]
[Description("Removes a hiking trail from the user's wishlist.")]
public (bool Success, string Message) RemoveHikingTrailFromWishlist(string id)
...

```

그리고 `HikingMate.WebApp/Components/Pages/ChatRoom.razor` 파일을 엽니다.

`@code` 영역의 `Load` 메서드 아래에 다음과 같은 코드를 추가해줍니다. `HikingMateWishlistService`를 생성해 플러그인으로 만들고 이를 커널에 추가합니다. 그러면 향후에 LLM이 사용자 입력에 적절하게 실행시킬 수 있는 함수를 찾아 적절하게 호출할 수 있습니다.

```csharp
var hikingWishlistService = new HikingMateWishlistService(wishlistItems);
hikingWishlistService.NeedUpdateUI += async (s, e) =>
{
    await InvokeAsync(StateHasChanged);
};

var wishlistPlugin = kernel.CreatePluginFromObject(hikingWishlistService);
kernel.Plugins.Add(wishlistPlugin);

```

`chatCompletionService`에서 `GetStreamingChatMessageContentsAsync`를 실행시킬 때 자동으로 함수가 호출되도록 설정해줘야 합니다. Send 메서드를 조금 수정하겠습니다.

```cs
private async Task Send()
{
    ...
    messages.Add(assistantMessage);

    //추가될 코드
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };


    //수정될 코드
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings: openAIPromptExecutionSettings, kernel);
}

```

그리고 UI도 수정해줍니다. 아래 코드가 있는 부분을 찾아 `추가되는 코드`를 추가해줍니다.

```html
...
<div>
    <button class="btn no-border" @onclick="@(() => TapWishlist())">
        <h3>위시리스트</h3>
    </button>
    <button class="btn no-border" style="margin-left:20px" @onclick="@(() => TapRecord())">
        <h3>기록</h3>
    </button>

    //추가되는 코드
    @if (!showRecord)
    {
        @if (wishlistExist)
        {
            <div class="wishlist-container">
                @foreach (var wishlistItem in wishlistItems)
                {
                    <div class="wishlist-item">
                        <p><b>@wishlistItem.Title</b></p>
                        <p>@wishlistItem.Description</p>
                    </div>
                }
            </div>
        }
        else
        {
            <div>
                <p>현재는 위시리스트가 없습니다.</p>
            </div>
        }
    }

...
```


코드 수정이 완료되었으면 다시 `dotnet run`을 터미널에서 실행합니다.
대화창에서 `서울 트래킹` 코스 등을 질문해보고, 위시리스트를 등록, 수정, 삭제가 모두 잘 되는 지 확인해봅니다.
잘 작동하지 않는다면 도움을 요청하세요.

축하합니다🎉  `함수 호출해보기(Function Calling)`를 완료하셨습니다. 이제 [STEP 05: 메모리 추가해보기](./step-05.md) 단계로 넘어가 보세요.