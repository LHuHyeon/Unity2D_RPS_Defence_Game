using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;

/*
 * File :   EnemyStat.cs
 * Desc :   몬스터 스탯
 *          몬스터의 능력치, 디버프, 피격 적용 등의 기능을 수행
 *
 & Functions
 &  [Public]
 &  : SetWaveStat()     - 웨이브에 맞게 스탯을 설정
 &  : OnAttacked()      - 피격 받을 시 디버프, 크리티컬 여부를 확인 후 적용
 &
 &  [private]
 &  : OnDeBuff()            - 디버프 활성화
 &  : DeBuffCoroutine()     - 디버프 코루틴
 &  : DamageTextEffect()    - 데미지 이펙트 생성
 *
 */

public class EnemyStat : MonoBehaviour
{
    [SerializeField] protected int              _id;
    [SerializeField] protected string           _name = "NoName";
    [SerializeField] protected Define.RaceType  _race;
    [SerializeField] protected int              _dropGold;      // 골드 드랍
    [SerializeField] protected int              _shield;        // 쉴드량
    [SerializeField] protected int              _hp;            // 체력
    [SerializeField] protected int              _defence;       // 방어력
    [SerializeField] protected float            _movespeed;     // 이동 속도
    [SerializeField] protected int              _maxHp;         // 최대 체력
    [SerializeField] protected int              _maxShield;     // 최대 쉴드량
    [SerializeField] protected int              _maxDefence;    // 최대 방어력
    [SerializeField] protected float            _maxMovespeed;  // 최대 이동 속도

    public int              Id              { get { return _id; }           set { _id = value; } }
    public string           Name            { get { return _name; }         set { _name = value; } }
    public Define.RaceType  Race            { get { return _race; }         set { _race = value; } }
    public int              DropGold        { get { return _dropGold; }     set { _dropGold = value; } }
    public int              Hp              { get { return _hp; }           set { _hp           = Mathf.Clamp(value, 0, MaxHp); } }
    public int              Shield          { get { return _shield; }       set { _shield       = Mathf.Clamp(value, 0, MaxShield); } }
    public int              Defence         { get { return _defence; }      set { _defence      = Mathf.Clamp(value, 0, MaxDefence); } }
    public float            MoveSpeed       { get { return _movespeed; }    set { _movespeed    = Mathf.Clamp(value, 0, MaxMoveSpeed); } }
    public int              MaxHp           { get { return _maxHp; }        set { _maxHp        = value; Hp = MaxHp; } }
    public int              MaxShield       { get { return _maxShield; }    set { _maxShield    = value; Shield = MaxShield; } }
    public int              MaxDefence      { get { return _maxDefence; }   set { _maxDefence   = value; Defence = MaxDefence; } }
    public float            MaxMoveSpeed    { get { return _maxMovespeed; } set { _maxMovespeed = value; MoveSpeed = MaxMoveSpeed; } }

    private bool            _isDebuffActive = false;
    private UI_HpBar        _hpBar;

    private Dictionary<Define.DeBuffType, DeBuff> Debuffs = new Dictionary<Define.DeBuffType, DeBuff>();

	enum DamageType
	{
		Default,
		Critical,
		Shield,
	}

    void Start()
    {
        _hpBar = GetComponent<EnemyController>()._hpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }

    // Wave에 맞게 스탯 수정
    public void SetWaveStat(WaveData waveData)
    {
        Race            = waveData.race;
        DropGold        = waveData.gold;
        
        MaxHp           = waveData.hp;
        MaxShield       = waveData.shield;
        MaxDefence      = waveData.defence;
        MaxMoveSpeed    = waveData.moveSpeed;

        // 고정 디버프 적용
        MaxDefence      = MaxDefence    - Mathf.RoundToInt(MaxDefence * Managers.Game.GetDebuff(Define.DeBuffType.DefenceDecrease));
        MaxShield       = MaxShield     - Mathf.RoundToInt(MaxShield * Managers.Game.GetDebuff(Define.DeBuffType.ShieldDecrease));
        MaxMoveSpeed    = MaxMoveSpeed  - (MaxMoveSpeed * Managers.Game.GetDebuff(Define.DeBuffType.Slow));

        if (_hpBar.IsNull() == false)
            _hpBar.RefreshUI();
    }

    // 공격 당하면
    public void OnAttacked(MercenaryStat stat, InstantBuffData deBuff = null)
    {
        if (stat.IsNull() == true)
            return;

        // 디버프 부여
        OnDeBuff(deBuff);

        // 쉴드가 존재하면 -1 차감 후 종료
        if (Shield > 0)
        {
            Shield--;
            DamageTextEffect(DamageType.Shield, 1);
            return;
        }

        int hitDamage = stat.Damage;

        // 추가 피해량 적용
        hitDamage = hitDamage + Mathf.RoundToInt(hitDamage * Managers.Game.HitDamageParcent);

        // 크리티컬 적용
        bool isCritical = Random.Range(1, 101) <= Managers.Game.CriticalParcent;
        if (isCritical == true)
            hitDamage = hitDamage + (int)(hitDamage * Managers.Game.CriticalDamageParcent);

        // 방어력은 공격력을 %만큼 흡수 [Damage(1000) * Defence(20)% = 800]
        hitDamage = hitDamage - Mathf.RoundToInt(hitDamage * (Defence * 0.01f));

        // 데미지 적용
        Hp -= hitDamage;

        DamageTextEffect(isCritical ? DamageType.Critical : DamageType.Default, hitDamage);

        if (Hp <= 0)
            GetComponent<EnemyController>().State = Define.State.Dead;
    }

    // Debuff 시작
    private void OnDeBuff(InstantBuffData deBuffData)
    {
        if (deBuffData.IsNull() == true)
            return;

        // 진행 중인 디버프가 존재 하는지 확인
        DeBuff debuff;
        if (Debuffs.TryGetValue(deBuffData.buffType, out debuff) == false)
        {
            debuff = new DeBuff();
            debuff._enemyStat = this;
            Debuffs.Add(deBuffData.buffType, debuff);
        }

        // 디버프 시작
        debuff.ApplyDebuff(deBuffData);

        // 쿨타임 시작
        if (_isDebuffActive == false)
            StartCoroutine(DeBuffCoroutine());
    }

    private IEnumerator DeBuffCoroutine()
    {
        _isDebuffActive = true;

        // 디버프가 존재하면 반복문 진행
        while(Debuffs.Count > 0)
        {
            // 받은 디버프 개수만큼 반복
            foreach(DeBuff debuff in Debuffs.Values)
            {
                // 디버프 쿨타임 계산
                debuff._elapsedTime -= Time.deltaTime;

                // 디버프가 끝나면 삭제
                if (debuff._elapsedTime <= 0)
                {
                    debuff.EndDebuff();
                    Debuffs.Remove(debuff._deBuffData.buffType);
                    break;
                }
            }

            yield return null;
        }

        _isDebuffActive = false;
    }

    // 데미지 텍스트 Effect 생성
    private void DamageTextEffect(DamageType damageType, int damage = 0)
    {
        DamageNumber damageNumber = Managers.Resource.Load<DamageNumber>("Prefabs/Text/"+damageType.ToString());
        damageNumber.Spawn(transform.position + (Vector3.up * 0.25f), damage);

        // 체력바 업데이트
        _hpBar.RefreshUI();

        // 보스라면
        if (Managers.Game.IsBoss == true)
            Managers.Game.GameScene.RefreshBossBar(this);
    }
}
