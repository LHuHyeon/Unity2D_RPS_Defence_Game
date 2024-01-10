using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   WarriorController.cs
 * Desc :   전사 컨트롤러
 *          전사만 사용가능한 능력을 구현합니다.
 *
 & Functions
 &  [Public]
 &  : SetStat() - 스탯 설정
 *
 */

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
