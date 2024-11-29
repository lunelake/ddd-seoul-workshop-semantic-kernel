# 🏞️HikingMate

HikingMate는 Open AI를 사용하고 대화를 통해 사용자가 하이킹 정보를 찾거나, 기록을 남기거나 하이킹 가고 싶은 Wishlist를 남기는 간단한 서비스입니다. 이 예제에는 Semantic Kernel을 통해 Function Calling을 하는 방법과 데이터를 임베딩하고 불러오는 코드가 포함되어 있습니다. 

*(2024년 8월 21일 Blazor Korea User Group의 BMW 밋업에서 `Semantic Kernel로 만드는 AI 앱`이라는 발표에서 사용된 예제 코드입니다.)*

### 프로젝트 설정

- 이 프로젝트는 Azure Open AI를 바탕으로 구성되어 있습니다. Azure에서 Open AI를 생성하고 `ApiKey`, `Endpoint`, `DeploymentName`을 HikingMate 프로젝트의 `Credential.cs` 파일에 입력해주어야 합니다. Open AI도 사용할 수 있지만 이를 위해서는 코드를 수정해야 합니다.
- 날씨를 질문하고 답을 받는 기능을 확인하기 위해서는 [OpenWeather]("https://openweathermap.org/")에서 발급받은 AppId가 있어야 합니다. 이 또한 `Credential.cs` 파일에 입력해주면 됩니다.

### Hiking Mate 테스트 해보기

시작 프로그램을 `HikingMate.WebApp`으로 놓고 실행하면 됩니다.


https://github.com/user-attachments/assets/251aaf5f-bb62-450e-99cf-4fa57ee6bcda


**해볼 수 있는 질문들**


1. 시스템 메시지 작동 확인
    - Q : 너는 누구니?
    - A : 안녕하세요! 😊 저는 하이킹 메이트예요. 여러분이 하이킹 트레일을 발견하고..
2. 저장되어 임베딩 되어 있는 데이터를 바탕으로 대답
    - Q : 내가 청계산 어떤 코스를 갔었지?
    - A : 🗓 날짜: 2024년 3월 3일 🏞 장소: 청계산 코스: 👉 원터골 코스 이 코스는 청계산의 대표적인 코스 중 하나로, 원터골 입구에서 시작하여 매봉 정상에 도달하는 코스로 총 거리는 약 4.7km입니다..
3.  Function Calling을 통해 날씨 정보 API를 호출해 대답
    - Q : 지금 청계산 날씨 어때?
    - A : 지금 청계산의 날씨는 다음과 같아요: 온도: 28.69°C 체감 온도: 33.31°C..
4. Function Calling을 ChatHistory에 있는 정보를 바탕으로 실행
    - Q : 서울에서 트래킹 할만한 곳은 어디야?
    - A : 서울에서 트래킹하기 좋은 곳들이 여러 곳 있어요! 여기 몇 가지 추천할 만한 곳들을 소개할게요. 🥾🌳 1. 북한산 (Bukhansan National Park) 코스: 도선사 코스, 우이암 코스, 정릉 코스 등..
    - Q : 너가 방금 소개해준 장소를 위시리스트에 추가해줘.
    - A : 모두 위시리스트에 성공적으로 추가했어요! 🎉.. 이제 서울에서 멋진 트래킹을 계획해보세요.
    - UI의 Wishlist에 여러 코스들 추가됨

5. 하이킹 기록 추가
    - Q : (ChatRoom.razor 파일에 주석되어 있는 있는 하이킹 기록) 기록으로 추가해줘.
    - A : 북한산 하이킹 기록이 성공적으로 저장되었습니다! 🏞️🎉
    - UI의 My hiking records에 기록이 추가됨




