# STEP 00: 개발 환경 설정 및 GPT 모델 사용 준비

이 단계에서는 워크샵 진행을 위해 필요한 개발 환경을 설정하고 GPT 모델 사용을 위한 준비를 합니다.

## GPT 모델 사용 준비

워크샵을 원활히 진행하려면 GPT 모델을 사용할 수 있어야 합니다. 모델은 Azure OpenAI, OpenAI, 또는 GitHub Models를 통해 생성할 수 있습니다. 만약 Azure나 OpenAI 계정이 없거나 모델 사용 비용이 부담스러우시다면 GitHub Models를 활용하실 수 있습니다. 워크샵은 기본적으로 GitHub Models에서 모델을 사용해 진행할 것이지만 Azure OpenAI, OpenAI에서 만든 모델도 Semantic Kernel에서 문제없이 사용할 수 있습니다.

### GitHub Models를 이용해 다양한 모델을 생성하고 연결

GitHub Models는 GitHub에서 제공하는 AI 모델 모음으로, 개발자들이 다양한 대규모 언어 모델(LLM)을 활용하여 지능형 애플리케이션을 구축할 수 있도록 지원합니다. 이러한 모델은 GitHub Marketplace에서 제공되며, Llama 3.1, GPT-4o, Phi-3.5 등과 같은 최신 AI 모델을 포함하고 있습니다. 개발자들은 GitHub의 내장 플레이그라운드를 통해 이러한 모델을 무료로 사용하고 테스트해볼 수 있습니다.

[GitHub Models](https://github.com/marketplace/models) 

### GitHub Models에서 PAT(Personal Access Token) 발급

1. [GitHub](https://github.com/)에 접속하여 계정에 로그인합니다.
2. 프로필 아이콘을 클릭하고 드롭다운 메뉴에서 **Settings**을 선택합니다.
3. 왼쪽 사이드 메뉴에서 **Developer settings**을 클릭합니다.
4. Personal Access Tokens 섹션 아래의 **Fine-grained tokens**을 클릭합니다.
5. **Generate new token** 버튼을 클릭합니다.
6. **Token name**에 토큰 이름(예:llm)을 입력하고 **Generate new token**을 눌러 PAT를 생성합니다.
7. 발급된 토큰을 복사해 안전한 곳에 붙여 넣어서 향후에 사용할 준비를 마칩니다.



## 개발 환경 설정

### 사전 준비 사항

- [Visual Studio Code](https://code.visualstudio.com) 설치
- [.NET SDK 9](https://dotnet.microsoft.com/ko-kr/download/dotnet/9.0) 설치
- [git CLI](https://git-scm.com/downloads) 설치
- [GitHub CLI](https://cli.github.com/) 설치

### VS Code에서 `C# Dev Kit` 설치 확인

- VS Code를 실행한 후, 좌측의 확장 아이콘을 클릭하거나 Ctrl+Shift+X를 눌러 확장 탭을 엽니다.
- 검색 창에 `C# Dev Kit`을 입력하고, Microsoft에서 제공하는 해당 확장을 찾아 '설치' 버튼을 클릭합니다.


### .NET SDK 설치 확인

VS Code를 실행합니다. 그리고 터미널에서 아래 명령어를 실행시켜 현재 .NET SDK를 설치했는지 확인합니다.

```
dotnet --list-sdks
```

`9.0.* [C:\Program Files\dotnet\sdk]`와 같은 식으로 터미널에서 출력되어야 합니다. 이와 같이 출력되지 않은 경우 [.NET SDK 설치 페이지](https://dotnet.microsoft.com/ko-kr/download/dotnet/9.0)에서 최신 버전을 다운로드 받아 설치합니다.


### git CLI 설치 확인

터미널에서 아래 명령어를 실행시켜 현재 git CLI를 설치했는지 확인합니다.

```
dotnet --list-sdks
```

만약 버전이 표시되지 않는다면 [git CLI](https://git-scm.com/downloads)에서 최신 버전을 다운로드 받아 설치합니다.


### GitHub CLI 설치 확인

터미널에서 아래 명령어를 실행시켜 현재 GitHub CLI를 설치했는지 확인합니다.

```
gh --version
```

만약 버전이 표시되지 않는다면 [GitHub CLI](https://cli.github.com/)에서 최신 버전을 다운로드 받아 설치합니다.


🎉 축하합니다! 개발 환경 설정이 완료되었습니다. 이제 [STEP 01: Semantic Kernel 사용해보기](./step-01.md) 단계로 넘어가 보세요.