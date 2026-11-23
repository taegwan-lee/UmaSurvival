# UmaSurvival

'CYGAMES'의 "우마무스메-프리티더비'를 모티브로 만든 Unity 2D 생존 액션 팬 프로젝트입니다. 

**Unity 6.2 (6000.2.9f1) · C# · URP 2D · Input System · TextMesh Pro**

## 실행 방법

1. 이 저장소를 클론하고, `Assets`, `Packages`, `ProjectSettings`가 들어 있는 폴더를 Unity Hub에 추가합니다.
2. Unity **6000.2.9f1**에서 프로젝트를 엽니다. 처음 열 때는 패키지와 에셋을 가져올 때까지 기다립니다.
3. [GameScene](Assets/Scenes/GameScene.unity)을 열고 **Play**를 누릅니다.

| 조작 | 입력 |
| --- | --- |
| 이동 | `W`, `A`, `S`, `D` |
| 공격 | 장착한 무기가 각자의 간격으로 자동 공격 |
| 강화 선택 | 레벨업 화면에서 카드 클릭 |
| 재시작 | 게임 오버 팝업에서 재시작 버튼 클릭 |

현재 빌드 진입점은 `GameScene`입니다. [TitleScene](Assets/Scenes/TitleScene.unity)과 [StoryScene](Assets/Scenes/StoryScene.unity)은 에디터에서 각각 열어 미리볼 수 있습니다.

## 구현한 기능

- **생존 전투** — 카메라 화면 바깥 네 방향에서 적이 등장해 플레이어를 추적합니다. 생존 시간이 늘어나면 생성 간격이 짧아지고, 빠른 적과 체력이 높은 적이 섞여 등장합니다.
- **경험치와 강화** — 적이 떨어뜨린 당근을 주우면 경험치를 얻습니다. 레벨업 시 전투가 멈추고 최대 세 장의 카드가 펼쳐집니다. 전역 공격력, 특정 무기의 공격 간격과 범위, 새 무기 획득 등을 선택할 수 있습니다.
- **HUD와 사망 연출** — 체력, 당근 마커로 표시한 경험치, 생존 시간을 보여줍니다. 체력이 0이 되면 사망 애니메이션과 슬로모션을 재생한 뒤 재시작 팝업을 띄웁니다.
- **타이틀과 프롤로그** — 타이틀에 로고 등장 연출과 구름 움직임을 적용했습니다. 프롤로그에서는 대사별 표정, 오디오, 캐릭터 퇴장을 설정할 수 있고, 현재 화자에 따라 캐릭터의 밝기가 달라집니다.
- **학원 앞마당** — 잔디와 흙길을 별도의 타일맵으로 배치했습니다. 흙길 Rule Tile은 이웃 칸에 맞춰 모서리와 끝 모양을 바꾸며, 시계탑이 있는 본관 스프라이트로 전투 필드의 배경을 구성했습니다.

### 무기

| 무기 | 동작 |
| --- | --- |
| 뒷발차기 | 왼쪽과 오른쪽을 번갈아 공격합니다. 애니메이션의 타격 프레임에 범위 피해를 적용합니다. |
| 밥그릇 투사체 | 사거리 안에서 가장 가까운 적을 찾아 그 방향으로 투사체를 발사합니다. |
| 편자 낙하 | 플레이어 주변의 무작위 지점에 떨어져 범위 피해를 줍니다. 전용 강화로 착지 지점에 지속 피해를 남깁니다. |
| 머리핀 투사체 | 플레이어가 마지막으로 이동한 방향으로 투사체를 발사하는 무기 구현입니다. |

## 구현 구조

| 영역 | 구현 방식 |
| --- | --- |
| 피해와 사망 | [Health](Assets/Scripts/Combat/Health.cs)가 체력과 무적 시간을 관리하고 사망 이벤트를 발행합니다. 플레이어의 사망 연출과 적의 경험치 드롭은 각 오브젝트의 사망 처리 컴포넌트에서 맡습니다. |
| 무기 추가와 보유 | [WeaponBase](Assets/Scripts/Combat/Weapons/WeaponBase.cs)가 공격 간격과 공통 능력치를 관리합니다. 각 무기는 발사나 타격 방식을 구현하고, [WeaponInventory](Assets/Scripts/Combat/Weapons/WeaponInventory.cs)는 보유 무기 조회와 중복 획득 검사를 맡습니다. |
| 강화 카드 | [UpgradeDefinition](Assets/Scripts/Combat/Upgrades/UpgradeDefinition.cs)은 카드별 효과와 최대 강화 횟수를 정의하는 ScriptableObject입니다. [UpgradeSystem](Assets/Scripts/Combat/Upgrades/UpgradeSystem.cs)은 이번 플레이의 강화 횟수를 보관하고, 보유 무기와 적용 조건을 확인해 카드 후보를 고릅니다. [카드 에셋 보기](Assets/Data/Upgrades) |
| 게임 상태와 연출 | [GameFlowController](Assets/Scripts/GameFlowController.cs)가 플레이, 강화 선택, 사망 연출, 게임 오버 상태에 따른 시간 배율을 관리합니다. [UpgradeSelectionUI](Assets/Scripts/UI/UpgradeSelectionUI.cs)는 게임 시간이 멈춰도 카드가 펼쳐지도록 `Time.unscaledDeltaTime`을 사용합니다. |
| 대화 데이터와 화면 | [StorySequence](Assets/Scripts/Story/StorySequence.cs)가 대사와 연출 데이터를 담습니다. [DialogueController](Assets/Scripts/Story/DialogueController.cs)는 대사 순서를 관리하고, [DialogueView](Assets/Scripts/Story/DialogueView.cs)는 텍스트와 캐릭터 연출을 표시합니다. [프롤로그 에셋 보기](Assets/Data/Prologue/Prologue.asset) |

## 프로젝트 폴더

- [Assets/Scenes](Assets/Scenes) — 타이틀, 프롤로그, 전투 씬
- [Assets/Scripts](Assets/Scripts) — 플레이어, 적, 전투, UI, 대화 스크립트
- [Assets/Prefabs](Assets/Prefabs) — 적, 무기, 투사체, 경험치 당근, 강화 카드
- [Assets/Data](Assets/Data) — 강화 카드와 프롤로그 데이터
- [Assets/Tilemaps](Assets/Tilemaps) — 바닥 타일, 흙길 Rule Tile, 타일 팔레트
