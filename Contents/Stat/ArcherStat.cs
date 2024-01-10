using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   ArcherStat.cs
 * Desc :   궁수의 스탯
 *          진화 여부에 따라 멀티샷 능력을 획득한다.
 *
 & Functions
 &  [Public]
 &  : RefreshAddData()  - 능력 새로고침
 &
 &  [protected]
 &  : OriginalBuff()    - 고정 버프 설정
 *
 */

public class ArcherStat : MercenaryStat
{
    public int      MaxMultiShotCount       { get; set; } = 0;
    public bool     IsMultiShot             { get; set; } = false;

    public override void RefreshAddData()
    {
        MaxMultiShotCount = 0;
        
        base.RefreshAddData();
    }

    // 고정 버프 설정
    protected override void OriginalBuff(BuffData buffData)
    {
        if ((buffData is OriginalBuffData) == false)
            return;

        base.OriginalBuff(buffData);

        OriginalBuffData buff = buffData as OriginalBuffData;

        switch(buff.buffType)
        {
            case Define.BuffType.MultiShot:
                MaxMultiShotCount  += (int)buff.value;
                IsMultiShot        = MaxMultiShotCount > 0;
                break;
            default:
                break;
        }
    }
}
