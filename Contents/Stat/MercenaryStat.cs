using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

/*
 * File :   MercenaryStat.cs
 * Desc :   용병 스탯
 */

public class MercenaryStat
{
    protected int                           _id;                    // 아이디
    protected string                        _name = "NoName";       // 이름
    protected int                           _salePrice;             // 판매 가격
    protected Define.RaceType               _race;                  // 종족
    protected Define.GradeType              _grade;                 // 등급
    protected Define.JobType                _job;                   // 직업
    protected GameObject                    _projectile;            // 발사체 Prefab
    protected GameObject                    _mercenary;             // 용병 Object
    protected SpriteLibraryAsset            _spriteLibrary;         // 캐릭터 파츠
    protected Sprite                        _icon;                  // 이미지
    protected RuntimeAnimatorController     _animatorController;    // 애니메이션 컨트롤러

    protected int                           _damage;                // 공격력
    protected float                         _attackRate;            // 공격 속도
    protected float                         _attackRange;           // 공격 사거리

    protected int                           _cureentEvolution;      // 현재 진화 단계

    public int                  Id              { get { return _id; }               set { _id = value; } }
    public string               Name            { get { return _name; }             set { _name = value; } }
    public int                  SalePrice       { get { return _salePrice; }        set { _salePrice = value; } }
    public Define.RaceType      Race            { get { return _race; }             set { _race = value; } }
    public Define.GradeType     Grade           { get { return _grade; }            set { _grade = value; } }
    public Define.JobType       Job             { get { return _job; }              set { _job = value; } }
    public GameObject           Projectile      { get { return _projectile; }       set { _projectile = value; } }
    public GameObject           Mercenary       { get { return _mercenary; }        set { _mercenary = value; } }
    public SpriteLibraryAsset   SpriteLibrary   { get { return _spriteLibrary; }    set { _spriteLibrary = value; }}
    public Sprite               Icon            { get { return _icon; }             set { _icon = value; }}

    public RuntimeAnimatorController    AnimatorController   { get { return _animatorController; }    set { _animatorController = value; }}

    public int      Damage                  { get { return _damage + AddDamage; }              set { _damage = value; } }
    public float    AttackRate              { get { return _attackRate + AddAttackRate; }      set { _attackRate = value; } }
    public float    AttackRange             { get { return _attackRange + AddAttackRange; }    set { _attackRange = value; } }

    public int      AddDamage               { get; set; } = 0;
    public float    AddAttackRate           { get; set; } = 0;
    public float    AddAttackRange          { get; set; } = 0;

    public int      MaxMultiShotCount       { get; set; } = 0;
    public bool     IsMultiShot             { get; set; } = false;

    public int      CureentEvolution        { get { return _cureentEvolution; } set { _cureentEvolution = value; }}
    public List<AbilityData> Abilities      { get; set; } = new List<AbilityData>();

    public void RefreshAddData()
    {
        AddDamage = 0;
        AddAttackRate = 0;
        AddAttackRange = 0;
        MaxMultiShotCount = 0;

        // 종족 강화 적용
        AddDamage += GetRaceAddDamage(_race);

        // 진화 능력 적용
        OnAbility();
    }

    // 진화에 따른 능력 적용
    private void OnAbility()
    {
        // 진화된 수만큼 능력 확인 후 적용
        for(int i=0; i<CureentEvolution; i++)
        {
            switch(Abilities[i].abilityType)
            {
                case Define.AbilityType.Damage:
                    AddDamage += (int)Abilities[i].value;
                    break;
                case Define.AbilityType.DamageParcent:
                    AddDamage += _damage / (int)(Abilities[i].value * 0.1f);
                    break;
                case Define.AbilityType.AttackRate:
                    AddAttackRate += Abilities[i].value;
                    break;
                case Define.AbilityType.AttackRange:
                    AddAttackRange += Abilities[i].value;
                    break;
                case Define.AbilityType.MultiShot:
                    MaxMultiShotCount  += (int)Abilities[i].value;
                    IsMultiShot        = MaxMultiShotCount > 0;
                    break;
                default:
                    break;
            }
        }
    }

    // 종족 강화에 따른 공격력 적용
	private int GetRaceAddDamage(Define.RaceType raceType)
	{
		switch (raceType)
		{
			case Define.RaceType.Human:
				return Managers.Game.HumanAddDamage;
			case Define.RaceType.Elf:
				return Managers.Game.ElfAddDamage;
			case Define.RaceType.WereWolf:
				return Managers.Game.WereWolfAddDamage;
			default:
				return 0;
		}
	}
}

/*
[ 능력, 강화 확인해야할 때 ]
1. 용병 소환 시 O
2. 용병 정보 Popup 호출 시
2-1. 진화에 대한 UI 표시하기 ( 버튼 누르면 뜨게끔? )


3. 진화 확인 시 슬롯에서 별 나오게 하기

*/