using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UpgradeData.cs
 * Desc :   강화 Data
 */
 
public class UpgradeData
{
    public int level;
    public int prime;           // 가격
    public int[] raceDamage = new int[((int)Define.RaceType.MaxMercenary)];
}
