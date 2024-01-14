using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   EnemyController.cs
 * Desc :   Monster 기본 기능
 *
 & Functions
 &  [Public]
 &  : SetWave()         - Wave에 맞는 몬스터 정보 설정
 &  : SetWayPoint()     - 생성 위치 설정
 &
 &  [Protected]
 &  : Init()            - 초기 설정
 &  : UpdateWalk()      - 움직일 때 (지정된 위치로 이동)
 &  : DeadCoroutine()   - 죽었을 때
 &
 &  [Private]
 &  : NextMoveTo()  - 다음 위치 설정
 &  : Clear()       - 초기화
 *
 */

public class EnemyController : BaseController
{
    private float           _moveSpeed;             // 속도
    private Vector3         _direction;             // 방향

    private int             _currentWayPointIndex;  // 현재 이동 위치 번호

    private Transform[]     _wayPoints;             // 이동할 위치들

    private EnemyStat       _stat;                  // 스탯

    public  UI_HpBar        _hpBar;
    public  bool            _isBoss = false;        // 보스인가?

    // Wave에 맞게 몬스터 정보 수정
    public void SetWave(WaveData waveData)
    {
        _stat.SetWaveStat(waveData);
        _spriteLibrary.spriteLibraryAsset = waveData.spriteLibrary;

        // 만약 보스라면 크기 2배로 키우기
        if (_isBoss == true && _hpBar.IsNull() == false)
            transform.localScale *= 2;
    }

    // 생성 위치 설정
    public void SetWayPoint(Transform[] wayPoints)
    {
        // 이동 위치 받기
        _wayPoints = new Transform[wayPoints.Length];
        _wayPoints = wayPoints;

        // 첫번째 위치로 이동
        _currentWayPointIndex = 0;
        transform.position = _wayPoints[_currentWayPointIndex].position;
    }

    protected override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Enemy;

        _stat = GetComponent<EnemyStat>();
    }

    protected override void UpdateIdle()
    {
        State = Define.State.Walk;
    }

    protected override void UpdateWalk()
    {
        transform.position += _direction * _stat.MoveSpeed * Time.deltaTime;

        // 도착할 위치에 0.1f 만큼 가까워지면 다음 위치 설정
        if ((_wayPoints[_currentWayPointIndex].position - transform.position).magnitude < 0.05f)
        {
            // 몬스터 위치를 정확하게 목표 위치로 설정
            transform.position = _wayPoints[_currentWayPointIndex].position;

            NextMoveTo();
        }
    }

    protected override IEnumerator DeadCoroutine()
    {
        GetComponent<Collider>().enabled = false;

        int gold = _stat.DropGold;

        // 확률적인 추가 골드
        if (Managers.Game.GoldParcent >= Random.Range(1, 101))
            gold += Managers.Game.AddGold;

        Managers.Game.GameGold += gold;

        yield return new WaitForSeconds(0.15f);

        GetComponent<Collider>().enabled = true;

        Clear();

        Managers.Game.Despawn(this.gameObject);

        coDead = null;
    }

    // 다음 위치 설정
    private void NextMoveTo()
    {
        // 다음 위치 Index + 1
        _currentWayPointIndex++;

        // 마지막 위치라면 2번째 위치로 이동 (1번째 위치는 생성 위치이기 때문)
        if (_currentWayPointIndex >= _wayPoints.Length)
            _currentWayPointIndex = 1;

        // 이동 방향 설정
        _direction = (_wayPoints[_currentWayPointIndex].position - transform.position).normalized;

        // 이동 방향 바라보기
        if (_direction == Vector3.left || _direction == Vector3.up)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            _hpBar.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (_direction == Vector3.right || _direction == Vector3.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _hpBar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Clear()
    {
        if (_isBoss == true)
        {
            _isBoss = false;
            Managers.Game.IsBoss = false;
            transform.localScale /= 2;
        }

        _stat.Hp    = _stat.MaxHp;
        _direction  = Vector3.zero;
        
        State = Define.State.Idle;
    }
}
