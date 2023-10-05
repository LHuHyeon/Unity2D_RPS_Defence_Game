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

	public enum StatType
	{
		MaxHp,
		WorkAbility,
		Likeability,
		Luck,
		Stress
	}

	public enum RewardType
	{
		Hp,
		WorkAbility,
		Likeability,
		Luck,
		Stress,
		Money,
		Block,
		SalaryIncrease,
		Promotion
	}

	public const string WaveDataNumber = "1279670946";
	public const string MercenaryDataNumber = "1983187163";
}
