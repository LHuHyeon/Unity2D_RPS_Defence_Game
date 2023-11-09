using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData
{
    public int level;
    public int prime;           // 가격
    public int[] raceDamage = new int[((int)Define.RaceType.MaxMercenary)];
}
