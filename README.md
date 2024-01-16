# [Unity 2D] RPS Defence Game
## 1. 소개

<div align="center">

  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/09e4606a-c51b-42d8-b2ab-04dc2c243251" width="30%" height="450"/>
  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/3fcad092-fd95-4c08-87ac-b6498a395df0" width="30%" height="450"/>
  <img src="https://github.com/LHuHyeon/LHuHyeon.github.io/assets/110723307/10c9c32b-8836-4e31-b709-29fe7f108f78" width="30%" height="450"/>

  < 게임 플레이 사진 >

</div>

+ Unity 2D Defence 게임입니다.

+ RPS는 가위바위보(Rock-Paper-Scissors)의 약자로 가위바위보 카드를 뽑아 용병을 획득하고 제한시간 안에 몬스터를 처치하는 모바일 Defnece 게임입니다.

+ 가위바위보 카드 중 똑같은 카드를 많이 뽑을 수록 높은 등급의 용병을 획득합니다.

+ 현재 Repository에는 유료에셋 사용으로 인해 소스코드만 등록되어 있습니다.

+ 개발기간: 2023.09.09 ~ 2023.11.20 ( 약 2개월 )

+ 형상관리: Git SourceTree

<br>

## 2. 개발 환경
+ Unity 2021.3.21f1 LTS

+ C#

+ Window 10

<br>

## 3. 사용 기술
| 기술 | 설명 |
|:---:|:---|
| 디자인 패턴 | ● **싱글톤** 패턴을 사용하여 Manager 관리 <br> ● **State** 패턴을 사용하여 캐릭터의 기능을 직관적으로 관리 |
| GoogleSheet | 구글 스프레드 시트를 사용해 데이터 관리 |
| Object Pooling | 자주 사용되는 객체는 Pool 관리하여 재사용 |
| Sprite Atlas | 자주 사용되는 Texture를 하나의 Texture로 묶어 드로우 콜 감소 |
| UI 자동화 | 유니티 UI 상에서 컴포넌트로 Drag&Drop되는 일을 줄이기 위한 편의성 |

<br>

## 4. 구현 컨텐츠
| 기능 | 설명 |
|:---:|:---|
| 용병 | ● 등급 : **일반 ~ 레전드리** 6개 등급이 존재하며 등급이 올라갈 수록 스탯 상승 <br> ● 종족 : **인간, 엘프, 웨어울프** 종족이 존재하며 종족별로 스탯의 장점이 존재 <br> ● 직업 : **전사, 궁수, 마법사** 직업이 존재하며 직업별로 다양한 진화 능력을 제공 |
| 진화 효과 | ● **전사** : 방어력 % 감소 부여 효과 <br> ● **궁수** : 다수를 공격하는 멀티샷 <br> ● **마법사** : 이동속도 % 감소 부여 효과와 스플래쉬 광역 공격 효과 |
| 능력 카드 | ● **Wave 7, 14, 21...에 도달하면 능력 카드 뽑기 가능** <br> ● 직업별 공격력 증가 ( 20% ~ 100% ) <br> ● 종족별 공격력 증가 ( 10% ~ 50% ) <br> ● 적 방어력 감소 ( 5% ~ 15% ) <br> ● 적 쉴드량 감소 ( 5% ~ 15% ) <br> ●  적 이동속도 감소 ( 5% ~ 15% ) <br> ● 적 피해량 증가 ( 5% ~ 15% ) <br> ● 적 처치 시 % 확률로 +1 골드 획득 ( 5% ~ 20% ) <br> ● 용병 치명타 확률 증가 ( 5% ~ 10% ) <br> ● 용병 치명타 피해량 증가 ( 20% ~ 100% ) <br> ● 용병 공격 범위 증가 ( 10% ~ 30% ) |
| Wave | ● Wave 총 50개 존재 <br> ● Wave가 증가할수록 몬스터 스탯도 상승 |
| 강화 | ● 획득한 골드로 종족별 공격력 강화 |

<br>

## 5. 개발 후 경험
| 기술 | 설명 |
|:---:|:---|
| PlayFab | 로그인과 회원가입 구현 경험 |
| Google Login | PlayFab과 연동하여 Google Login 구현 경험 [(코드)](https://github.com/LHuHyeon/Unity2D_RPS_Defence_Game/blob/main/Scripts/Manager/PlayFabManager.cs) |

<br>

## 6. 게임 다운로드
+ https://drive.google.com/drive/folders/1MrBZm8UblWOneZTP1ARweFM5INbB2C4p?usp=sharing
