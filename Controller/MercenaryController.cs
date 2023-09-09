using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryController : BaseController
{
    private int                 _maks = (1 << (int)Define.LayerType.Enemy);

    [SerializeField]
    private float               attackRate;     // 공격 속도
    [SerializeField] 
    private float               attackRange;    // 공격 사거리

    private Transform           attackTarget;   // 공격 대상

    private AttackAnimHandler   attackHandler;  // 공격 처리기

    private MercenaryStat       _stat;          // 스탯
    private EnemyController     enemy;          // 적 정보

    protected override void Init()
    {
        base.Init();

        attackHandler = Utils.FindChild<AttackAnimHandler>(this.gameObject);
    }

    protected override void UpdateIdle()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, _maks);

        foreach(Collider collider in colliders)
        {
            if (attackTarget.IsNull() == true)
            {
                attackTarget = collider.transform;
                State = Define.State.Attack;
                return;
            }
        }
    }

    protected override void UpdateAttack()
    {
        // 공격 사거리 체크
        float distance = (attackTarget.position - transform.position).magnitude;
        if (distance > attackRange)
        {
            StopAttack();
            return;
        }

        LookTarget();       // 타겟 바라보기
    }

    protected override void UpdateDead()
    {
        
    }

    // 공격 중단
    private void StopAttack()
    {
        attackTarget = null;
        State = Define.State.Idle;
    }

    // 타겟 바라보기
    private void LookTarget()
    {
        // 이동 방향 설정
        Vector3 direction = (attackTarget.position - transform.position).normalized;

        // 0보다 작으면 타겟이 왼쪽에 있고
        if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        // 0보다 크면 타겟이 오른쪽에 있는 것
        else if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
