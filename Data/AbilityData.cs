using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   AbilityData.cs
 * Desc :   능력 Data
 *
 & Functions
 &  [Public]
 &  : AbilityClone() - 능력 복사 (DeepCopy)
 *
 */
 
public class AbilityData
{
    public Define.AbilityType   abilityType;    // 능력 타입
    public List<int>            values;         // 값
    public int                  currentValue;   // 현재 값
    public string               name;           // 이름
    public string               descripition;   // 설명

    public AbilityData AbilityClone()
    {
        return new AbilityData()
        {
            abilityType     = this.abilityType,
            values          = this.values,
            currentValue    = this.currentValue,
            name            = this.name,
            descripition    = this.descripition,
        };
    }
}
