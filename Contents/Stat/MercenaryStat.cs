using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   MercenaryStat.cs
 * Desc :   용병 스탯
 */

public class MercenaryStat : Stat
{
    [SerializeField] protected int          _damage;        // 공격력
    [SerializeField] protected float        _attackRate;    // 공격 속도
    [SerializeField] protected float        _attackRange;   // 공격 사거리
    [SerializeField] protected GameObject   _projectile;    // 발사체 Sprite

    public int          Damage          { get { return _damage; }       set { _damage = value; } }
    public float        AttackRate      { get { return _attackRate; }   set { _attackRate = value; } }
    public float        AttackRange     { get { return _attackRange; }  set { _attackRange = value; } }
    public GameObject   Projectile      { get { return _projectile; }   set { _projectile = value; } }
}
