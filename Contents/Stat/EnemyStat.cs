using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
몬스터가 피격받았을 때 그리고 죽었을 때를 관리한다.

Collider를 통해 피격받고 있으므로 죽으면 끄기.
*/

/*
 * File :   EnemyStat.cs
 * Desc :   적 스탯
 */

public class EnemyStat : Stat
{
    [SerializeField] protected int      _hp;            // 체력
    [SerializeField] protected int      _maxHp;         // 최대 체력
    [SerializeField] protected int      _defence;       // 방어력
    [SerializeField] protected int      _dropGold;      // 골드 드랍
    [SerializeField] protected float    _movespeed;     // 이동 속도

    public int      Hp          { get { return _hp; }           set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int      MaxHp       { get { return _maxHp; }        set { _maxHp = value; Hp = MaxHp; } }
    public int      Defence     { get { return _defence; }      set { _defence = value; } }
    public int      DropGold    { get { return _dropGold; }     set { _dropGold = value; } }
    public float    MoveSpeed   { get { return _movespeed; }    set { _movespeed = value; } }

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
        if (damage > 0)
            Hp -= damage;

        // TODO : Hit Text Effect 생성

        // 체력이 0보다 작으면 사망
        if (Hp <= 0)
        {
            GetComponent<EnemyController>().State = Define.State.Dead;
        }
    }
}
