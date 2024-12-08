# STEP 05: 메모리 추가해보기

HikingMate에는 메모리 서비스가 있습니다. 사용자가 하이킹 한 후기를 올리면 이를 벡터화 시켜 임베딩 시킵니다. 그러면 사용자는 후에 질의를 할 때 `북한산 언제 갔다왔지`와 같은 질문을 통해 북한산을 다녀온 날짜를 찾고 기억을 회상할 수 있습니다.

Semantic Kernel은 Vector DB를 지원하는 각종 DB를 지원합니다. 하지만 이번 연습에서는 인 메모리를 활용해 임베딩 시나리오를 처리합니다.

먼저 `Services` 폴더에 `HikingmateRecordService.cs` 파일을 추가 하고, 아래의 코드를 복사하겠습니다.
아래의 코드를 통해 `ISemanticKernelTextMomory`를 구현한 개체를 생성합니다. 그리고 임베딩에는 OpenAI의 `text-embedding-3-large` 모델을 사용합니다.

```cs
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System.ComponentModel;

public class HikingmateRecordService
{
    public ISemanticTextMemory Memory { get; set; }

    public string MemoryCollectionName = "memory-collection";

    List<HikingRecord> records;
    public event EventHandler? NeedUpdateUI;

    public HikingmateRecordService(List<HikingRecord> records)
    {

        var modelId = "text-embedding-3-large";
        var client = GitHubModelsClient.GetClient();

        var memory = new MemoryBuilder()
            .WithTextEmbeddingGeneration(new OpenAITextEmbeddingGenerationService(modelId, client))
            .WithMemoryStore(new VolatileMemoryStore())
            .Build();

        Memory = memory;

        this.records = records;
    }    
}

```

그리고 Record 데이터를 DB에 저장하고 임베딩 시켜 VectorDB에 저장해보겠습니다.

```cs
[KernelFunction]
[Description("Saves a hiking record with details such as title, date and content.")]
public async Task SaveHikingRecord(string title, string date, string content)
{
    records.Add(new HikingRecord { Title = title, Date = date, Content = content });
    NeedUpdateUI?.Invoke(this, new EventArgs());

    var text = @$"제목 : {title}
날짜 : {date}
내용 : {content}";

    await Memory.SaveInformationAsync(MemoryCollectionName, text, Guid.NewGuid().ToString());
}

```
그리고 테스트를 위해 초기 데이터도 코드에서 생성해서 넣어주겠습니다.

```cs
public async Task Load()
{
    var title = "청계산 하이킹";
    var date = "2024년 3월 3일";
    var content = @"날씨: 날씨는 맑았고, 부드러운 바람이 불어 하이킹하기에 최적의 날씨였습니다.

코스:
이번 하이킹에서는 원터골 코스를 선택했습니다. 이 코스는 청계산 하이킹의 대표적인 코스 중 하나로, 원터골 입구에서 시작하여 매봉 정상에 도달하는 코스입니다. 총 거리는 약 4.7km로, 왕복 약 2시간 30분에서 3시간 정도가 소요됩니다. 원터골 코스는 초보자부터 중급자까지 모두에게 적합한 코스로, 산길이 비교적 완만하고, 등산로가 잘 정비되어 있어 걷기에 편리합니다.

경험:
하이킹은 아침 일찍 시작했습니다. 원터골 입구에서부터 시작된 등산로는 소나무와 참나무가 우거져 있어, 처음부터 숲의 신선한 공기를 느낄 수 있었습니다. 등산로 초반은 완만한 오르막길로, 가볍게 몸을 풀며 걷기에 좋았습니다. 길을 따라 올라가다 보면 여러 가지 운동기구와 휴게소가 있어 잠시 쉬어가기 좋습니다.

중간 지점에는 청계산 약수터가 있어, 시원한 물을 마시며 잠시 휴식을 취했습니다. 이곳은 등산객들이 많이 모이는 곳으로, 다들 물을 마시며 에너지를 보충하고 있었습니다. 약수터를 지나 더 올라가면 경사가 조금 더 가파라지지만, 경치가 아름다워 오르며 지루하지 않았습니다.

하이킹 중반쯤, 매봉에 가까워지면서 주변 풍경이 점점 더 웅장해졌습니다. 이 구간에서 바라보는 서울 도심의 풍경은 매우 인상적이었습니다. 정상에 가까워질수록 코스는 조금 더 힘들어지지만, 나무데크로 된 계단이 잘 설치되어 있어 안전하게 올라갈 수 있었습니다.

마침내 매봉 정상에 도착했을 때, 청계산의 아름다움과 함께 서울의 전경을 한눈에 담을 수 있었습니다. 정상에서 바라보는 풍경은 정말 놀라웠고, 잠시 이곳에서 휴식을 취하며 간단한 간식과 함께 경치를 즐겼습니다.

하산은 오를 때보다 더 수월했습니다. 내려오는 길에는 옥녀봉을 지나쳐 원터골 입구로 다시 돌아왔습니다. 하산하는 동안에도 길게 뻗은 나무와 깨끗한 숲길을 따라 걸으며 마음의 안정을 느낄 수 있었습니다.

소감:
청계산 하이킹은 도시에서 벗어나 자연 속에서 에너지를 충전할 수 있는 멋진 경험이었습니다. 원터골 코스는 비교적 쉬운 편이지만, 정상에서의 보람 있는 풍경과 청량한 공기 덕분에 몸과 마음이 모두 상쾌해졌습니다. 시간이 부족해 멀리 가지 못하는 분들에게도 강력히 추천할 만한 코스입니다.";

    await SaveHikingRecord(title, date, content);
}

```

그리고 임베딩 된 데이터를 검색하는 코드도 추가해주겠습니다.

``` cs

[KernelFunction]
[Description("Retrieves hiking records")]
public async Task<List<MemoryQueryResult>> GetHikingRecords(string query)
{
    var memory = Memory;
    var memories = memory.SearchAsync(MemoryCollectionName, query, 3, 0);

    return await memories.ToListAsync();
}

```

다음 `HikingMate.WebApp/Components/Pages/ChatRoom.razor` 파일을 엽니다.

`@code` 영역의 `Load` 메서드 아래에 다음과 같은 코드를 추가해줍니다. `HikingmateRecordService`를 생성해 플러그인으로 만들고 이를 커널에 추가합니다. 그러면 향후에 LLM이 사용자 입력에 적절하게 실행시킬 수 있는 함수를 찾아 적절하게 호출할 수 있습니다.

```csharp

var hikingmateRecord = new HikingmateRecordService(records);
hikingmateRecord.NeedUpdateUI += async (s, e) =>
{
    await InvokeAsync(StateHasChanged);
};

await hikingmateRecord.Load();

var recordPlugin = kernel.CreatePluginFromObject(hikingmateRecord);
kernel.Plugins.Add(recordPlugin);

```

그리고 UI도 수정해줍니다. 아래 코드가 있는 부분을 찾아 `추가되는 코드`를 추가해줍니다.

```html
...
    ...
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
    //추가되는 코드
    else
    {
        @if (recordsExist)
        {
            <div class="wishlist-container">
                @foreach (var record in records)
                {
                    <div class="wishlist-item">  
                        <p><b>@record.Title</b></p>  
                        <p>@record.Date</p>  
                        <div style="display: -webkit-box; -webkit-box-orient: vertical; -webkit-line-clamp: 3; overflow: hidden; text-overflow: ellipsis;">  
                            @record.Content  
                        </div>  
                    </div>
                }
            </div>
        }
        else
        {
            <div>
                <p>현재는 기록이 없습니다.</p>
            </div>
        }
    }

...
```

코드 수정이 완료되었으면 다시 `dotnet run`을 터미널에서 실행합니다.
기록 탭을 클랙해 `청계산 하이킹`이라는 제목의 기록이 제대로 표시되는 지 확인합니다. 표시되지 않으면 도움을 요청하세요.

대화창에서 `청계산 언제 갔다왔어`를 질문해보면 아래와 같이 기록된 내용을 바탕으로 답변을 합니다.

```
청계산 하이킹은 2024년 3월 3일에 다녀오셨네요! 🏞️ 날씨가 맑고, 부드러운 바람이 불어 하이킹하기에 최적의 날씨였다고 기록되어 있어요. 하이킹은 아침 일찍 시작했고, 원터골 코스를 선택하셨다고 하네요. 이 코스는 청계산 하이킹의 대표적인 코스 중 하나로, 초보자부터 중급자까지 모두에게 적합한 코스라고 해요. 청계산 정상에서 바라보는 서울의 전경이 아주 인상적이었다고 하셨네요. 😍 하산길에서는 벚꽃과 함께 걸으며 마음의 안정을 느낄 수 있었다니, 정말 좋은 경험이었을 것 같아요! 🌸 이 외에도 더 궁금한 점이나 또 다른 하이킹 기록이 필요하시면 언제든지 말씀해 주세요! 😊
```

그리고 다음 내용을 대화에 입력해봅니다.

```
북한산 하이킹 기록이야. 추가해줘.

날짜: 2024년 7월 3일

장소: 대한민국 북한산

날씨: 맑고 쾌청한 날씨로, 하이킹하기에 이상적인 조건이었습니다.

코스:
이번 하이킹에서는 북한산의 대표적인 코스인 백운대 코스를 선택했습니다. 이 코스는 북한산국립공원 내에서도 가장 인기 있는 코스 중 하나로, 도선사 입구에서 출발해 백운대 정상에 오르는 코스입니다. 왕복 거리는 약 7.4km, 총 소요 시간은 4시간에서 5시간 정도입니다. 초반은 비교적 완만한 편이지만, 후반으로 갈수록 경사가 급해져 난이도가 중상급으로 평가됩니다.

경험:
하이킹은 이른 아침부터 시작했습니다. 도선사 입구에서 출발해 초반에는 산길이 완만하고 비교적 걷기 쉬웠습니다. 길을 따라 걷다 보면 울창한 숲이 양옆으로 펼쳐져 있어, 신선한 공기와 함께 자연을 느낄 수 있었습니다. 이른 시간이라 등산로는 한적했고, 새소리와 바람 소리만이 들려오는 고요한 산의 분위기를 즐길 수 있었습니다.

중간 지점에 도달할 때쯤, 도선사에 잠시 들렀습니다. 도선사는 북한산을 대표하는 사찰로, 사찰 주변의 평온한 분위기와 아름다운 건축물이 인상적이었습니다. 잠시 도선사에서 쉬며, 차분한 마음으로 다시 등산을 시작했습니다.

도선사를 지나면서부터 경사가 점점 가팔라지기 시작했습니다. 백운대 피난소를 지나면서부터는 바위 구간이 많아지며, 손을 사용해 암벽을 타고 오르는 구간도 있었습니다. 이곳은 체력과 주의력이 요구되지만, 곳곳에 설치된 안전장비가 있어 비교적 안전하게 오를 수 있었습니다.

마침내 백운대 정상에 도착했을 때, 북한산의 전경과 서울 시내를 한눈에 바라볼 수 있었습니다. 정상에서의 뷰는 정말 웅장하고, 산과 도시가 한눈에 들어오는 멋진 풍경이었습니다. 바람이 시원하게 불어와 피로가 싹 가시는 느낌이 들었습니다. 정상에서 잠시 쉬며 준비한 간식을 먹고, 사진도 찍으며 여유로운 시간을 보냈습니다.

하산은 구기동 방향으로 진행했습니다. 내려오는 길은 오를 때만큼의 체력은 필요 없었지만, 돌이 많은 구간이 있어 발목에 주의해야 했습니다. 내려오면서도 북한산의 아름다운 자연을 느낄 수 있었고, 구기동에 도착해서는 주변의 한적한 분위기 속에서 하루를 마무리했습니다.

소감:
북한산 하이킹은 도심에서 가까우면서도 자연의 웅장함을 느낄 수 있는 특별한 경험이었습니다. 백운대 코스는 비교적 도전적인 구간이 있어 하이킹을 즐기는 이들에게 매우 추천할 만한 코스입니다. 정상에서의 경치와 하산 후의 뿌듯함은 말로 표현할 수 없을 만큼 멋진 경험이었습니다. 자연 속에서 에너지를 충전하고 싶은 분들에게 북한산을 강력히 추천드립니다.

```

그러면 리뷰를 성공적으로 임베딩하고 추가한 것을 알 수 있습니다. 문제가 있으면 도움을 요청하세요.


축하합니다🎉  `Semantic Kernel과 Blazor로 만드는 나만의 AI 앱 워크샵`의 모든 내용을 완료하셨습니다. 수고 많으셨습니다.