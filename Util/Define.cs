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
}
