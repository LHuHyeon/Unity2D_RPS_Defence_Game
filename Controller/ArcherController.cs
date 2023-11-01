using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MercenaryController
{
    protected ArcherStat        _archerStat;    // 궁수 스탯

    protected int               _currentMultiShotCount  = 0;
    protected List<Transform>   _multiShotTargets       = new List<Transform>();

    public override void SetStat(MercenaryStat stat)
    {
        _archerStat = stat.MercenaryClone<ArcherStat>();
        _stat = _archerStat;

        base.SetStat(_stat);
    }

    protected override void StartAttackEvent()
    {
        base.StartAttackEvent();

        if (_archerStat.IsMultiShot == true)
            StartMultiShot();
    }

    protected override void StopAttack()
    {
        base.StopAttack();

        _multiShotTargets.Clear();
        _currentMultiShotCount = 0;
    }
    
#region 멀티샷

    // 여러 타겟 공격 시작
    private void StartMultiShot()
    {
        // 여러 적 탐지
        TargetsDetection();

        // 탐지된 적들 공격
        for(int i=0; i<_multiShotTargets.Count; i++)
        {
            Transform target = _multiShotTargets[i];

            // 거리 체크 or 주 공격 대상인지 or 존재 여부 체크
            if (IsAttackRangeCheck(target) == false || target == _mainAttackTarget || target.gameObject.isValid() == false)
            {
                --_currentMultiShotCount;
                _multiShotTargets.Remove(target);
                continue;
            }
            
            // 공격 시작
            StartAttack(target);
        }
    }
    
    // 여러 적 탐지
    private void TargetsDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _archerStat.AttackRange, _mask);
        
        foreach(Collider collider in colliders)
        {
            // 목표물인지 확인 (첫 공격 대상인지?)
            if (collider.transform == _mainAttackTarget)
                continue;

            // 이미 탐지된 적인지 확인
            if (_multiShotTargets.Contains(collider.transform) == true)
                continue;

            // 탐지 개수 확인
            if (_currentMultiShotCount >= _archerStat.MaxMultiShotCount)
                return;

            _currentMultiShotCount++;
            _multiShotTargets.Add(collider.transform);
        }
    }

#endregion
}
