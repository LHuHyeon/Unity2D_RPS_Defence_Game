using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
몬스터 전용 스크립트이다.
몬스터는 정해진 장소로 이동하게 된다.

좌표는 몬스터 생성 스크립트(SpawningPool)에서 받을 수 있다.
*/

/*
 * File :   MonsterController.cs
 * Desc :   Monster 기본 기능
 *
 & Functions
 &  [Public]
 &  : SetUp()       - 기능 설정
 &
 &  [Protected]
 &  : UpdateWalk()  - 움직일 때 (지정된 위치로 이동)
 &  : UpdateDead()  - 죽었을 때
 &
 &  [Private]
 &  : NextMoveTo()  - 다음 위치 설정
 *
 */

public class EnemyController : BaseController
{
    private float           _moveSpeed;             // 속도
    private Vector3         _direction;             // 방향

    private int             currentWayPointIndex;   // 현재 이동 위치 번호
    private Transform[]     _wayPoints;             // 이동할 위치들

    private MonsterStat     _stat;                  // 스탯

    // 세팅
    public void SetUp(Transform[] wayPoints)
    {
        _wayPoints = new Transform[wayPoints.Length];
        _wayPoints = wayPoints;

        currentWayPointIndex = 0;
        transform.position = _wayPoints[currentWayPointIndex].position;

        _moveSpeed = GetComponent<MonsterStat>().MoveSpeed;

        if (_anim.IsNull() == true)
            _anim = GetComponent<SPUM_Prefabs>()._anim;

        State = Define.State.Walk;
    }

    protected override void UpdateWalk()
    {
        transform.position += _direction * _moveSpeed * Time.deltaTime;

        // 도착할 위치에 0.1f 만큼 가까워지면 다음 위치 설정
        if ((_wayPoints[currentWayPointIndex].position - transform.position).magnitude < 0.01f)
        {
            // 몬스터 위치를 정확하게 목표 위치로 설정
            transform.position = _wayPoints[currentWayPointIndex].position;

            NextMoveTo();
        }
    }

    protected override void UpdateDead()
    {
        /*
        1초 뒤에 없애기
        */
    }

    // 다음 위치 설정
    private void NextMoveTo()
    {
        // 다음 위치 Index + 1
        currentWayPointIndex++;

        // 마지막 위치라면 2번째 위치로 이동 (1번째 위치는 생성 위치이기 때문)
        if (currentWayPointIndex >= _wayPoints.Length)
            currentWayPointIndex = 1;

        // 이동 방향 설정
        _direction = (_wayPoints[currentWayPointIndex].position - transform.position).normalized;

        // 이동 방향 바라보기
        if (_direction == Vector3.left || _direction == Vector3.up)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_direction == Vector3.right || _direction == Vector3.down)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
