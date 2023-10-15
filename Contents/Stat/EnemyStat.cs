using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;

/*
 * File :   EnemyStat.cs
 * Desc :   적 스탯
 */

public class EnemyStat : MonoBehaviour
{
    [SerializeField] protected int              _id;
    [SerializeField] protected string           _name = "NoName";
    [SerializeField] protected Define.RaceType  _race;
    [SerializeField] protected int              _hp;            // 체력
    [SerializeField] protected int              _maxHp;         // 최대 체력
    [SerializeField] protected int              _defence;       // 방어력
    [SerializeField] protected int              _maxDefence;    // 최대 방어력
    [SerializeField] protected int              _dropGold;      // 골드 드랍
    [SerializeField] protected float            _movespeed;     // 이동 속도

    public int              Id          { get { return _id; }           set { _id = value; } }
    public string           Name        { get { return _name; }         set { _name = value; } }
    public Define.RaceType  Race        { get { return _race; }         set { _race = value; } }
    public int              Hp          { get { return _hp; }           set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int              MaxHp       { get { return _maxHp; }        set { _maxHp = value; Hp = MaxHp; } }
    public int              Defence     { get { return _defence; }      set { _defence = Mathf.Clamp(value, 0, MaxDefence); } }
    public int              MaxDefence  { get { return _maxDefence; }   set { _maxDefence = value; Defence = MaxDefence; } }
    public int              DropGold    { get { return _dropGold; }     set { _dropGold = value; } }
    public float            MoveSpeed   { get { return _movespeed; }    set { _movespeed = value; } }

    private UI_HpBar _hpBar;

    void Start()
    {
        _hpBar = GetComponent<EnemyController>()._hpBar = Managers.UI.MakeWorldSpaceUI<UI_HpBar>(transform);
    }

    // Wave에 맞게 스탯 수정
    public void SetWaveStat(WaveData waveData)
    {
        Race        = waveData.race;
        MaxHp       = waveData.hp;
        Defence     = waveData.defence;
        MoveSpeed   = waveData.moveSpeed;
        DropGold    = waveData.gold;

        if (_hpBar.IsNull() == false)
            _hpBar.RefreshUI();
    }

    // 공격 당하면
    public void OnAttacked(int damage)
    {
        if (damage <= 0)
            return;

        // 방어력이 존재하면 -1 차감 후 종료
        if (Defence > 0)
        {
            Defence--;
            return;
        }

        int hitDamage = damage;

        // TODO : 100 랜덤 수 중 10 이하면 크리티컬! (나중에 크리티컬 확률 완성 시 수정)
        bool isCritical = Random.Range(0, 101) < 10;
        if (isCritical == true)
            hitDamage = damage + (int)(damage / 1.5);

        Hp -= hitDamage;

        DamageText(isCritical, hitDamage);  // 데미지 텍스트 생성
        _hpBar.RefreshUI();                  // 체력바 설정

        if (Hp <= 0)
        {
            GetComponent<EnemyController>().State = Define.State.Dead;
        }
    }

    // 데미지 텍스트 Effect 생성
    private void DamageText(bool isCritical, int damage)
    {
        DamageNumber damageNumber;
        if (isCritical == true)
            damageNumber = Managers.Resource.Load<DamageNumber>("Prefabs/Text/Critical");
        else
            damageNumber = Managers.Resource.Load<DamageNumber>("Prefabs/Text/Default");
        
        damageNumber.Spawn(transform.position + (Vector3.up * 0.25f), damage);
    }
}
