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
    protected float                         _attackSpeed;            // 공격 속도
    protected float                         _attackRange;           // 공격 사거리

    protected Define.EvolutionType          _cureentEvolution = Define.EvolutionType.Unknown;   // 현재 진화 단계

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
    public float    AttackSpeed             { get { return _attackSpeed + AddAttackRate; }     set { _attackSpeed = value; } }
    public float    AttackRange             { get { return _attackRange + AddAttackRange; }    set { _attackRange = value; } }

    public int      AddDamage               { get; set; } = 0;
    public float    AddAttackRate           { get; set; } = 0;
    public float    AddAttackRange          { get; set; } = 0;

    public int      MaxMultiShotCount       { get; set; } = 0;
    public bool     IsMultiShot             { get; set; } = false;

    public Define.EvolutionType     CurrentEvolution    { get { return _cureentEvolution; } set { _cureentEvolution = value; }}
    public List<AbilityData>        Abilities           { get; set; } = new List<AbilityData>();

    // 들어온 용병 정보와 내 정보가 같은지 확인
    public bool IsSameMercenary(MercenaryStat mercenary, bool isEvolution = true)
    {
        if (Id != mercenary.Id)
            return false;

        // 진화 정보가 같은지?
        if (isEvolution == true)
        {
            if (CurrentEvolution == mercenary.CurrentEvolution)
                return true;
        }
        else
        {
            if (CurrentEvolution == Define.EvolutionType.Unknown)
                return true;
        }

        return false;
    }

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
        for(int i=0; i<((int)CurrentEvolution); i++)
        {
            switch(Abilities[i].abilityType)
            {
                case Define.AbilityType.Damage:
                    AddDamage += (int)Abilities[i].value;
                    break;
                case Define.AbilityType.DamageParcent:
                    AddDamage += Mathf.RoundToInt(Damage * (Abilities[i].value * 0.01f));
                    break;
                case Define.AbilityType.AttackSpeed:
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

    // 깊은 복사 (DeepCopy)
    public MercenaryStat MercenaryClone()
    {
        return new MercenaryStat()
        {
            Id                  = this.Id,
            Name                = this.Name,
            SalePrice           = this.SalePrice,
            Race                = this.Race,
            Grade               = this.Grade,
            Job                 = this.Job,
            Projectile          = this.Projectile,
            Mercenary           = this.Mercenary,
            SpriteLibrary       = this.SpriteLibrary,
            Icon                = this.Icon,
            AnimatorController  = this.AnimatorController,
            Damage              = this.Damage,
            AttackSpeed         = this.AttackSpeed,
            AttackRange         = this.AttackRange,
            AddDamage           = this.AddDamage,
            AddAttackRate       = this.AddAttackRate,
            AddAttackRange      = this.AddAttackRange,
            MaxMultiShotCount   = this.MaxMultiShotCount,
            IsMultiShot         = this.IsMultiShot,
            CurrentEvolution    = this.CurrentEvolution,
            Abilities           = this.Abilities,
        };
    }
}