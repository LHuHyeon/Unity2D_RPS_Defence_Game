using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   WizardController.cs
 * Desc :   마법사 컨트롤러
 *          마법사가 사용가능한 능력을 구현합니다.
 *
 & Functions
 &  [Public]
 &  : SetStat() - 스탯 설정
 *
 */
 
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
