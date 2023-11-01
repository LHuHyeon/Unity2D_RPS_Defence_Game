using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MercenaryController
{
    protected WarriorStat _warriorStat;    // 궁수 스탯

    public override void SetStat(MercenaryStat stat)
    {
        _warriorStat = stat.MercenaryClone<WarriorStat>();
        _stat = _warriorStat;

        base.SetStat(_stat);
    }
}
