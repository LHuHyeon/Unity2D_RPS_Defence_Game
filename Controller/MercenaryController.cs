using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   MercenaryController.cs
 * Desc :   용병의 기본 기능
 *
 & Functions
 &  [Public]
 &  : SetStat()         - 생성 설정
 &
 &  [Protected]
 &  : Init()            - 초기 설정
 &  : UpdateIdle()      - 멈춤일 때 (주변 적 탐색)
 &  : UpdateAttack()    - 공격할 때 (적 생존 확인 후 바라보기)
 &
 &  [Private]
 &  : StartAttackEvent()    - 공격 시작 Animation Event
 &  : StopAttackEvent()     - 공격 중단 Animation Event
 &  : IsAttackRangeCheck()  - 대상과 거리 확인
 &  : LookTarget()          - 대상 바라보기
 &  : StopAttack()          - 공격 중단
 *
 */

public class MercenaryController : BaseController
{
    private int                 _mask = (1 << (int)Define.LayerType.Enemy);

    private Transform           _attackTarget;  // 공격 대상

    private MercenaryStat       _stat;          // 스탯
    private EnemyController     enemy;          // 적 정보

    public Tile                 currentTile;    // 현재 타일

    public MercenaryStat GetMercenaryStat() { return _stat; }

    // 생성 시 설정
    public void SetStat(MercenaryStat stat)
    {
        _stat = stat;
        _spriteLibrary.spriteLibraryAsset = _stat.SpriteLibrary;
        _anim.runtimeAnimatorController = stat.AnimatorController;
        _stat.Mercenary = this.gameObject;

        State = Define.State.Idle;
    }

    protected override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Mercenary;
    }

    protected override void UpdateIdle()
    {
        // 주변 Enemy 탐색
        Collider[] colliders = Physics.OverlapSphere(transform.position, _stat.AttackRange, _mask);

        // Enemy 감지 시 공격
        foreach(Collider collider in colliders)
        {
            _attackTarget = collider.transform;
            enemy = _attackTarget.GetComponent<EnemyController>();

            // 거리 한번 더 체크 후 공격 진행
            if (IsAttackRangeCheck() == true)
                State = Define.State.Attack;

            return;
        }
    }

    protected override void UpdateAttack()
    {
        // 대상이 없거나 죽었을 시 Idle
        if (_attackTarget.gameObject.isValid() == false || enemy.State == Define.State.Dead)
        {
            State = Define.State.Idle;
            return;
        }
        
        // 타겟 바라보기
        LookTarget();
    }

    // 공격 시작 [ Animation Event ]
    private void StartAttackEvent()
    {
        if (_attackTarget.IsFakeNull() == true)
            return;

        // 발사체 생성
        GameObject projectile = Managers.Resource.Instantiate(_stat.Projectile);

        // 발사!
        projectile.transform.position = transform.position;
        projectile.GetComponent<Projectile>().SetTarget(_attackTarget, _stat.Damage);
    }

    // 공격 중단 [ Animation Event ]
    private void StopAttackEvent()
    {
        // 대상이 공격 사거리 안에 있으면 Attack
        if (IsAttackRangeCheck() == true)
            State = Define.State.Attack;
        else
            State = Define.State.Idle;
    }

    // 타겟과의 거리 체크
    private bool IsAttackRangeCheck()
    {
        if (_attackTarget.IsFakeNull() == true)
            return false;

        // 공격 사거리 체크
        float distance = (_attackTarget.position - transform.position).magnitude;
        if (distance > _stat.AttackRange)
        {
            StopAttack();
            return false;
        }

        return true;
    }

    // 타겟 바라보기
    private void LookTarget()
    {
        // 회전 방향 설정
        Vector3 direction = (_attackTarget.position - transform.position).normalized;

        // 0보다 작으면 타겟이 왼쪽에 있기 때문에 왼쪽 바라보기
        if (direction.x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        // 0보다 크면 타겟이 오른쪽에 있기 때문에 오른쪽 바라보기
        else if (direction.x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // 공격 중단
    private void StopAttack()
    {
        _attackTarget = null;
        State = Define.State.Idle;
    }
}
