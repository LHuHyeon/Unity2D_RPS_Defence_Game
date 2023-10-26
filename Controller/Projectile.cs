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
    private float           _attackSpeed = 7f;
    private MercenaryStat   _stat;

    private Transform       _attackTarget;

    public void SetTarget(Transform target, MercenaryStat stat)
    {
        _attackTarget = target;
        _stat = stat;
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
            // TODO : 접촉 후 또 다른 효과 확인 (폭발, 감전 등)
            _attackTarget.GetComponent<EnemyStat>().OnAttacked(_stat.Damage, _stat.DebuffAbility);
            Managers.Resource.Destroy(this.gameObject);
        }
    }
}
