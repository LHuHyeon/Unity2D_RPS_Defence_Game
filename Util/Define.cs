using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Define
{
	public enum UIEvent
	{
		Click,
		Pressed,
		PointerDown,
		PointerUp,
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
	}

	public enum WorldObject
	{
		Unknown,
		Mercenary,
		Enemy,
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
		Unknown	= 0,
		Human	= 1,	// 인간
		Elf		= 2,	// 엘프
		Goblin	= 3,	// 고블린
		Undead	= 4,	// 언데드
		Devil	= 5,	// 악마
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
}
