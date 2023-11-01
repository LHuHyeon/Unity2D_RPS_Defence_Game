using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MercenaryController
{
    protected WizardStat _wizardStat;    // 궁수 스탯

    public override void SetStat(MercenaryStat stat)
    {
        _wizardStat = stat.MercenaryClone<WizardStat>();
        _stat = _wizardStat;

        base.SetStat(_stat);
    }
}
