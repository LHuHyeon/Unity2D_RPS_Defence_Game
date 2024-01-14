using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Define
{
	public enum UIEvent
    {
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
    }

	public enum Scene
	{
		Unknown,
		Dev,
		Game,
		Login,
		Loby,
		Load,
	}

	public enum Sound
	{
		Bgm,
		Effect,
		Speech,
		Max,
	}

	public enum State
	{
		None,
		Idle,
		Walk,
		Attack,
		Dead,
	}

	public enum LayerType
	{
		Mercenary 	= 6,
		Enemy		= 7,
		Tile		= 8,
	}

	public enum WorldObject
	{
		Unknown,
		Mercenary,
		Enemy,
	}

	// 능력 카드
	public enum AbilityType
	{
		Unknown				= 0,

		WarriorDamage		= 1,	// 직업 공격력 	% 상승
		ArcherDamage		= 2,
		WizardDamage		= 3,

		HumanDamage			= 4,	// 종족 공격력 	% 상승
		ElfDamage			= 5,
		WereWolfDamage		= 6,

		DefenceDecrease 	= 7,	// 적 방어력 	% 감소
		ShieldDecrease 		= 8,	// 적 쉴드 		% 감소
		SpeedDecerase		= 9,	// 적 이동속도	% 감소

		GoldParcent 		= 10,	// % 확률로 +1 Gold 획득
		HitDamage			= 11,	// 피해량 % 증가
		CriticalParcent		= 12,	// 크리티컬 확률% 증가
		CriticalDamage		= 13,	// 크리티컬 데미지 % 증가
		AttackRange			= 14,	// 공격 범위% 증가

		Max 				= 15,
	}

	// 버프 타입
	public enum BuffType
	{
		Damage			= 1,	// 공격력
		DamageParcent 	= 2,	// 공격력 % 증가
		AttackSpeed		= 3,	// 공격 속도
		AttackRange		= 4,	// 공격 사거리
		MultiShot		= 5,	// 멀티샷
		Splash			= 6,	// 스플래쉬
	}

	// 디버프 타입
	public enum DeBuffType
	{
		DefenceDecrease	= 1,	// 방어력 감소
		Slow			= 2,	// 이동속도 감소
		Stun			= 3,	// 기절/경직
		ShieldDecrease	= 4,	// 쉴드량 감소
	}

	public enum EvolutionType
	{
		Unknown	= 0,
		Star1	= 1,
		Star2	= 2,
		Star3	= 3,
		Max		= 4,
	}

	public enum RPSCard
	{
		Unknown,
		Rock		= 1,	// 주먹
		Scissors	= 2,	// 가위
		Paper		= 3,	// 보
		Max,
	}

	public enum GradeType
	{
		Basic		= 0,	// E 기본
		Common		= 1,	// D 일반
		UnCommon	= 2,	// C 고급
		Rare		= 3,	// B 희귀
		Epic		= 4,	// A 영웅
		Legendary	= 5,	// S 전설
	}

	public enum JobType
	{
		Unknown	= 0,
		Warrior	= 1,	// 전사
		Archer	= 2,	// 궁수
		Wizard	= 3,	// 마법사
		Max		= 4,
	}

	public enum RaceType
	{
		Unknown			= 0,
		Human			= 1,	// 인간
		Elf				= 2,	// 엘프
		WereWolf		= 3,	// 웨어 울프
		MaxMercenary 	= 4,

		Goblin			= 4,	// 고블린
		Undead			= 5,	// 언데드
		Devil			= 6,	// 악마
	}

	// * Google Sheet 데이터 ID
	public const string StartDataNumber			= "883832159";
	public const string MercenaryDataNumber 	= "1983187163";
	public const string UpgradeDataNumber 		= "2068092916";
	public const string EvolutionDataNumber 	= "279655201";
	public const string OriginalBuffDataNumber 	= "1888981556";
	public const string InstantBuffDataNumber	= "1364019981";
	public const string AbilityDataNumber		= "275010908";
	
	public const string Stage01DataNumber		= "1279670946";

	// * 최대값 최솟값
	public const int MIN_PASSWORD_LENGTH = 8;
	public const int MAX_PASSWORD_LENGTH = 18;

	// * Regex 정규식
	public const string RegexUserName	= @"^[0-9a-z]{2,8}$";
	public const string	RegexEmail		= @"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$";

	public const string SaleConfirmText = "판매하시겠습니까?";

	// * Text Data ID
	public const int Login			= 10000;
	public const int SignUp			= 10001;
	public const int GoogleLogin	= 10002;

	public const int LoginFalse			= 10010;
	public const int UserNameLength		= 10020;
	public const int EmailCheckFalse	= 10030;
	public const int PasswordLength		= 10040;
	public const int PasswordCheckFalse	= 10041;
}
