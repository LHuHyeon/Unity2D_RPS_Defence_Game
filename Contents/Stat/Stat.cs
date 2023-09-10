using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Stat.cs
 * Desc :   공통 스탯
 */

public class Stat : MonoBehaviour
{
    [SerializeField] protected int      _id;
    [SerializeField] protected string   _name = "NoName";

    public int      Id      { get { return _id; }    set { _id = value; } }
    public string   Name    { get { return _name; }  set { _name = value; } }
}
