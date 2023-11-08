using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardStat : MercenaryStat
{
    public float    SplashRange { get; set; } = 0;      // 스플래쉬 범위
    public bool     IsSplash    { get; set; } = false;  // 스플래쉬 여부

    public override void RefreshAddData()
    {
        SplashRange = 0;
        
        base.RefreshAddData();
    }

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
