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
    [SerializeField] protected int              _dropGold;      // 골드 드랍
    [SerializeField] protected float            _movespeed;     // 이동 속도

    public int              Id          { get { return _id; }           set { _id = value; } }
    public string           Name        { get { return _name; }         set { _name = value; } }
    public Define.RaceType  Race        { get { return _race; }         set { _race = value; } }
    public int              Hp          { get { return _hp; }           set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int              MaxHp       { get { return _maxHp; }        set { _maxHp = value; Hp = MaxHp; } }
    public int              Defence     { get { return _defence; }      set { _defence = value; } }
    public int              DropGold    { get { return _dropGold; }     set { _dropGold = value; } }
    public float            MoveSpeed   { get { return _movespeed; }    set { _movespeed = value; } }

    // Wave에 맞게 스탯 수정
    public void SetWaveStat(WaveData waveData)
    {
        Race        = waveData.race;
        MaxHp       = waveData.hp;
        Defence     = waveData.defence;
        MoveSpeed   = waveData.moveSpeed;
        DropGold    = waveData.gold;
    }

    // 공격 당하면
    public void OnAttacked(int damage)
    {
        if (damage <= 0)
            return;

        int hitDamage = damage;

        // 100 랜덤 수 중 10 이하면 크리티컬! (TODO : 임시 코드)
        bool isCritical = Random.Range(0, 101) < 10;
        if (isCritical == true)
            hitDamage = damage + (int)(damage / 1.5);

        Hp -= hitDamage;
        DamageText(isCritical, hitDamage);

        // 체력이 0보다 작으면 사망
        if (Hp <= 0)
        {
            GetComponent<EnemyController>().State = Define.State.Dead;
        }
    }

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
