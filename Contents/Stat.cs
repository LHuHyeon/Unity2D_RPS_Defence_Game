using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] protected int _id;
    [SerializeField] protected string _name = "NoName";
    [SerializeField] protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _dropGold;
    [SerializeField] protected float _movespeed;

    public int Id { get { return _id; } set { _id = value; } }
    public string Name { get { return _name; } set { _name = value; } }
    public int Hp { get { return _hp; } set { _hp = Mathf.Clamp(value, 0, MaxHp); } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; Hp = MaxHp; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int DropGold { get { return _dropGold; } set { _dropGold = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }
}
