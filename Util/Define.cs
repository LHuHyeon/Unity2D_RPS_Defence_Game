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

	// 능력 종류
	public enum AbilityType
	{
		Unknown			= 0,
		Buff			= 100,	// [ 자신 버프 구간 ]
		Damage			= 101,	// 공격력
		DamageParcent 	= 102,	// 공격력 % 증가
		AttackSpeed		= 103,	// 공격 속도
		AttackRange		= 104,	// 공격 사거리

		DeBuff			= 200,	// [ 몬스터 디버프 구간 ]
		DefenceDecrease	= 201,	// 방어력 감소
		Slow			= 202,	// 이동속도 감소
		Stun			= 203,	// 기절/경직

		Skill			= 300,	// [ 스킬 구간 ]
		MultiShot		= 301,	// 멀티샷
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
	}

	public enum RaceType
	{
		Unknown		= 0,
		Human		= 1,	// 인간
		Elf			= 2,	// 엘프
		WereWolf	= 3,	// 웨어 울프
		Goblin		= 4,	// 고블린
		Undead		= 5,	// 언데드
		Devil		= 6,	// 악마
	}

	public const string WaveDataNumber = "1279670946";
	public const string MercenaryDataNumber = "1983187163";
	public const string UpgradeDataNumber = "2068092916";
	public const string EvolutionDataNumber = "279655201";

	public const string SaleConfirmText = "판매하시겠습니까?";
}
