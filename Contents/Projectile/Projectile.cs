using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Projectile.cs
 * Desc :   발사체 기능
 *
 & Functions
 &  [Public]
 &  : SetTarget()   - 대상 설정
 &
 &  [Private]
 &  : TargetChase() - 대상 추격
 *
 */

public class Projectile : MonoBehaviour
{
    protected   MercenaryStat   _stat;

    private     float           _attackSpeed = 7f;
    private     Transform       _attackTarget;

    public void SetTarget(Transform target, MercenaryStat stat)
    {
        _attackTarget = target;
        _stat = stat;

        GetComponent<SpriteRenderer>().sprite = _stat.ProjectileIcon;
    }

    void FixedUpdate()
    {
        TargetChase();
    }

    // 대상 추격
    private void TargetChase()
    {
        if (_attackTarget.gameObject.isValid() == false)
        {
            Managers.Resource.Destroy(this.gameObject);
            return;
        }

        // 방향 구하기
        Vector3 direction   = (_attackTarget.position - transform.position).normalized;

        // 방향으로 이동
        transform.position  += direction * _attackSpeed * Time.deltaTime;

        // 대상 바라보기
        float angle         = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation  = Quaternion.AngleAxis(angle, Vector3.forward);

        // 접촉하면 공격
        if ((_attackTarget.position - transform.position).magnitude < 0.3f)
        {
            TouchAbility();
            _attackTarget.GetComponent<EnemyStat>().OnAttacked(_stat.Damage, _stat.DebuffAbility);
            Managers.Resource.Destroy(this.gameObject);
        }
    }

    // 접촉할 경우 실행되는 능력
    protected virtual void TouchAbility() {}
}
