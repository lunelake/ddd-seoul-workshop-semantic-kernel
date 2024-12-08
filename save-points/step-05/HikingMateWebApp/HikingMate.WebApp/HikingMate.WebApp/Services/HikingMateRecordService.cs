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
    
    [KernelFunction]
    [Description("Retrieves hiking records")]
    public async Task<List<MemoryQueryResult>> GetHikingRecords(string query)
    {
        var memory = Memory;
        var memories = memory.SearchAsync(MemoryCollectionName, query, 3, 0);
    
        return await memories.ToListAsync();
    }
}