using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   WizardStat.cs
 * Desc :   마법사의 스탯
 *          진화 여부에 따라 스플래쉬 능력을 획득한다.
 *
 & Functions
 &  [Public]
 &  : RefreshAddData()  - 능력 새로고침
 &
 &  [protected]
 &  : OriginalBuff()    - 고정 버프 설정
 *
 */

public class WizardStat : MercenaryStat
{
    public float    SplashRange { get; set; } = 0;      // 스플래쉬 범위
    public bool     IsSplash    { get; set; } = false;  // 스플래쉬 여부

    public override void RefreshAddData()
    {
        SplashRange = 0;
        
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
            case Define.BuffType.Splash:
                SplashRange += buff.value;
                IsSplash    = SplashRange > 0f;
                break;
            default:
                break;
        }
    }
}
