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
    protected int                           _id;
    protected string                        _name = "NoName";
    protected Define.RaceType               _race;
    protected int                           _salePrice;             // 판매 가격
    protected Define.GradeType              _grade;                 // 등급
    protected Define.JobType                _job;                   // 직업
    protected SpriteLibraryAsset            _spriteLibrary;         // 캐릭터 파츠
    protected Sprite                        _icon;                  // 이미지
    protected GameObject                    _projectile;            // 발사체 Prefab
    protected Sprite                        _projectileIcon;        // 발사체 이미지

    protected int                           _damage;                // 공격력
    protected float                         _attackSpeed;           // 공격 속도
    protected float                         _attackRange;           // 공격 범위

    protected Define.EvolutionType          _cureentEvolution = Define.EvolutionType.Unknown;   // 현재 진화 단계

    public int                  Id              { get { return _id; }               set { _id = value; } }
    public string               Name            { get { return _name; }             set { _name = value; } }
    public Define.RaceType      Race            { get { return _race; }             set { _race = value; } }
    public int                  SalePrice       { get { return _salePrice; }        set { _salePrice = value; } }
    public Define.GradeType     Grade           { get { return _grade; }            set { _grade = value; } }
    public Define.JobType       Job             { get { return _job; }              set { _job = value; } }
    public SpriteLibraryAsset   SpriteLibrary   { get { return _spriteLibrary; }    set { _spriteLibrary = value; }}
    public Sprite               Icon            { get { return _icon; }             set { _icon = value; }}
    public GameObject           Projectile      { get { return _projectile; }       set { _projectile = value; }}
    public Sprite               ProjectileIcon  { get { return _projectileIcon; }   set { _projectileIcon = value; }}

    public float    AttackSpeed             { get { return _attackSpeed + AddAttackRate; }          set { _attackSpeed = value; } }
    public float    AttackRange             { get { return _attackRange + AddAttackRange; }         set { _attackRange = value; } }
    public int      Damage
    {
        get { return _damage + AddDamage + AddRaceDamage + AddAbilityDamage; }
        set { _damage = value; } 
    }

    public int      AddDamage               { get; set; } = 0;
    public int      AddRaceDamage           { get; set; } = 0;
    public int      AddAbilityDamage        { get; set; } = 0;
    public float    AddAttackRate           { get; set; } = 0;
    public float    AddAttackRange          { get; set; } = 0;

    public Define.EvolutionType     CurrentEvolution    { get { return _cureentEvolution; } set { _cureentEvolution = value; }}
    public List<BuffData>           Buffs               { get; set; } = new List<BuffData>();

    public InstantBuffData          DebuffAbility       { get; set; }

    public MercenaryController  _mercenary;

    // 들어온 용병 정보와 내 정보가 같은지 확인
    public bool IsSameMercenary(MercenaryStat mercenary, bool isEvolution = true)
    {
        if (Id != mercenary.Id)
            return false;

        // 진화 정보를 확인할 것인가?
        if (isEvolution == true)
        {
            // 진화 정보가 같은지 확인
            if (CurrentEvolution == mercenary.CurrentEvolution)
                return true;
        }
        else
        {
            // 진화가 안된 상태인지 확인
            if (CurrentEvolution == Define.EvolutionType.Unknown)
                return true;
        }

        return false;
    }

    public virtual void RefreshAddData()
    {
        StatClear();

        // 종족 강화 데미지
        AddRaceDamage += Managers.Game.GetRaceAddDamage(_race);

        // 진화 능력 적용
        OnAbility();

        AddAttackRange += Managers.Game.AddAttackRange;

        AddAbilityDamage += Mathf.RoundToInt(Damage * (Managers.Game.GetRaceAddDamageParcent(_race) * 0.01f));
        AddAbilityDamage += Mathf.RoundToInt(Damage * (Managers.Game.GetJobAddDamageParcent(_job) * 0.01f));

        // 객체 존재 시 스탯 새로고침
        if (_mercenary.IsNull() == false)
            _mercenary.RefreshObject();
    }

    // 진화에 따른 능력 적용
    private void OnAbility()
    {
        // 진화된 수만큼 능력 확인 후 적용
        for(int i=0; i<((int)CurrentEvolution); i++)
        {
            BuffData buff = Buffs[i];

            OriginalBuff(buff);     // 고정 버프 능력 확인
            InstantBuff(buff);      // 일시 버프 확인
        }
    }

    // 고정적인 효과를 주는 버프
    protected virtual void OriginalBuff(BuffData buffData)
    {
        if ((buffData is OriginalBuffData) == false)
            return;

        OriginalBuffData buff = buffData as OriginalBuffData;

        switch(buff.buffType)
        {
            case Define.BuffType.Damage:
                AddDamage += (int)buff.value;
                break;
            case Define.BuffType.DamageParcent:
                AddDamage += Mathf.RoundToInt(Damage * (buff.value * 0.01f));
                break;
            case Define.BuffType.AttackSpeed:
                AddAttackRate += buff.value;
                break;
            case Define.BuffType.AttackRange:
                AddAttackRange += buff.value;
                break;
            default:
                break;
        }
    }

    // 일시적인 효과를 주는 버프or디버프
    private void InstantBuff(BuffData buffData)
    {
        if ((buffData is InstantBuffData) == false)
            return;

        InstantBuffData buff = buffData as InstantBuffData;

        switch(buff.buffType)
        {
            case Define.DeBuffType.DefenceDecrease:
            case Define.DeBuffType.Slow:
            case Define.DeBuffType.Stun:
                DebuffAbility = buff;
                break;
        }
    }

    // 깊은 복사 (DeepCopy)
    public T MercenaryClone<T>() where T : MercenaryStat, new()
    {
        return new T()
        {
            Id                  = this.Id,
            Name                = this.Name,
            SalePrice           = this.SalePrice,
            Race                = this.Race,
            Grade               = this.Grade,
            Job                 = this.Job,
            SpriteLibrary       = this.SpriteLibrary,
            Icon                = this.Icon,
            Projectile          = this.Projectile,
            ProjectileIcon      = this.ProjectileIcon,
            Damage              = this.Damage - this.AddDamage - this.AddRaceDamage - this.AddAbilityDamage,
            AttackSpeed         = this.AttackSpeed - this.AddAttackRate,
            AttackRange         = this.AttackRange - this.AddAttackRange,
            CurrentEvolution    = this.CurrentEvolution,
            Buffs               = this.Buffs,
            DebuffAbility       = this.DebuffAbility,
        };
    }

    private void StatClear()
    {
        AddDamage = 0;
        AddRaceDamage = 0;
        AddAbilityDamage = 0;
        AddAttackRate = 0;
        AddAttackRange = 0;
    }
}