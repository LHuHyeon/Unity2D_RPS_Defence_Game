using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherStat : MercenaryStat
{
    public int      MaxMultiShotCount       { get; set; } = 0;
    public bool     IsMultiShot             { get; set; } = false;

    public override void RefreshAddData()
    {
        MaxMultiShotCount = 0;
        
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
            case Define.BuffType.MultiShot:
                MaxMultiShotCount  += (int)buff.value;
                IsMultiShot        = MaxMultiShotCount > 0;
                break;
            default:
                break;
        }
    }
}
