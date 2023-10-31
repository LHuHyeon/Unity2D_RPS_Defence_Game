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

    private Transform           _mainAttackTarget;  // 주 공격 대상

    private MercenaryStat       _stat;              // 스탯
    private EnemyController     _enemy;             // 적 정보

    private int                 _currentMultiShotCount  = 0;

    private List<Transform>     _multiShotTargets = new List<Transform>();

    private UI_EvolutionBar     _evolutionBar;

    public MercenaryStat GetStat() { return _stat; }

    // 생성 시 설정
    public void SetStat(MercenaryStat stat)
    {
        _stat = stat;
        _spriteLibrary.spriteLibraryAsset = _stat.SpriteLibrary;
        _anim.runtimeAnimatorController = stat.AnimatorController;
        _stat.Mercenary = this.gameObject;

        _stat.RefreshAddData();

        _evolutionBar = Managers.UI.MakeWorldSpaceUI<UI_EvolutionBar>(transform);
        _evolutionBar.SetInfo(_stat);

        _anim.SetFloat("AttackSpeed", _stat.AttackSpeed);

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
            _mainAttackTarget = collider.transform;
            _enemy = _mainAttackTarget.GetComponent<EnemyController>();

            // 거리 한번 더 체크 후 공격 진행
            if (IsAttackRangeCheck(_mainAttackTarget) == true)
                State = Define.State.Attack;

            return;
        }
    }

    protected override void UpdateAttack()
    {
        // 대상이 없거나 죽었을 시 Idle
        if (_mainAttackTarget.gameObject.isValid() == false || _enemy.State == Define.State.Dead)
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
        if (_mainAttackTarget.IsFakeNull() == true)
            return;

        StartAttack(_mainAttackTarget);

        if (_stat.IsMultiShot == true)
            StartMultiShot();
    }

    // 공격 중단 [ Animation Event ]
    private void StopAttackEvent()
    {
        // 대상이 공격 사거리 안에 있으면 Attack
        if (IsAttackRangeCheck(_mainAttackTarget) == true)
            State = Define.State.Attack;
        else
            StopAttack();
    }

    // 공격 시작
    private void StartAttack(Transform target)
    {
        // 발사체 생성
        GameObject projectile = Managers.Resource.Instantiate(_stat.Projectile);

        // 발사!
        projectile.transform.position = transform.position;
        projectile.GetComponent<Projectile>().SetTarget(target, _stat);
    }

    // 타겟과의 거리 체크
    private bool IsAttackRangeCheck(Transform target)
    {
        if (target.IsFakeNull() == true)
            return false;

        // 공격 사거리 체크
        float distance = (target.position - transform.position).magnitude;
        if (distance > _stat.AttackRange)
            return false;

        return true;
    }

    // 타겟 바라보기
    private void LookTarget()
    {
        // 회전 방향 설정
        Vector3 direction = (_mainAttackTarget.position - transform.position).normalized;

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
        _mainAttackTarget = null;
        State = Define.State.Idle;

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, _stat.AttackRange, _mask);
        
        foreach(Collider collider in colliders)
        {
            // 목표물인지 확인 (첫 공격 대상인지?)
            if (collider.transform == _mainAttackTarget)
                continue;

            // 이미 탐지된 적인지 확인
            if (_multiShotTargets.Contains(collider.transform) == true)
                continue;

            // 탐지 개수 확인
            if (_currentMultiShotCount >= _stat.MaxMultiShotCount)
                return;

            _currentMultiShotCount++;
            _multiShotTargets.Add(collider.transform);
        }
    }

#endregion
}
