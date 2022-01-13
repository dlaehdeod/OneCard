<pre>
# One-Card
[Unity Version 2018.4.36f1] 

[게임에 사용한 음악은 저작권이 있기 때문이 표기를 해둡니다.]
Music name : Cute (www.bensound.com), (https://www.bensound.com/royalty-free-music/track/cute)

[파일 소개]
1. OneCard_Play : 유니티로 빌드 된 실행파일입니다.

2. OneCard_Unity : 유니티 프로젝트 전체 내용입니다.
                   코드는 OneCard_Unity -> Assets -> Scripts 에 위치합니다.

[게임 코드 간단한 소개]

1. BackgroundMusic.cs : 싱글톤을 이용하여 배경음악을 담당합니다. 

2. Option.cs : 플레이어 수, 시작 카드 수, 패배 카드 수를 기록하고, 스크롤바 및 버튼을 이용하는 옵션 뷰를 담당합니다. 

3. SevenCard.cs : 원카드에서 7카드는 카드의 문양을 원하는대로 바꿀 수 있습니다.
                  카드 문양 선택 뷰를 별도로 만들어주어 관리하게 만들었습니다.

4. TurnManager.cs : 플레이어의 차례 및 게임오버에 대한 로직을 담당합니다.

5. CardManager.cs : 카드에 관한 모든 로직을 담당합니다.
                    시작 및 카드 나눠주기, 낼 수 있는 유효한 카드 체크, 공격 카드 체크,
                    카드 섞기, 카드 정리 등 게임의 핵심 로직을 담고 있습니다.
                    플레이어를 제외한 모든 컴퓨터 AI도 담당하고 있습니다.

6. PlayerController.cs : 플레이어 차례일 때 마우스 혹은 터치에 대한 상호작용을 담당합니다.
                         카드를 옮기는 기능이 들어있습니다.
</pre>
