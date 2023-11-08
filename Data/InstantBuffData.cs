using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일시적인 버프
public class InstantBuffData : BuffData
{
    public bool                     isDeBuff;       // 디버프인가?
    public Define.DeBuffType        buffType;       // 버프 종류
    public int                      parcentage;     // 확률
    public float                    time;           // 시간
}
