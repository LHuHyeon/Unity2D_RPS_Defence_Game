using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
모든 컨트롤러의 부모이다. (몬스터, 플레이어, 용병 등..)
기본적인 상태와 필요한 변수를 가지고 있다.
*/

public abstract class BaseController : MonoBehaviour
{
    protected Animator _anim;

    [SerializeField]
    private Define.State _state = Define.State.Idle;    // 캐릭터 상태
    public Define.State State
    {
        get { return _state; }
        set {
            _state = value;

            switch (_state)
            {
                case Define.State.Idle:
                    _anim.CrossFade("Idle", 0.4f);
                    break;
                case Define.State.Walk:
                    _anim.CrossFade("Walk", 0.1f);
                    break;
                case Define.State.Attack:
                    _anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case Define.State.Dead:
                    _anim.CrossFade("Dead", 0.1f, -1, 0);
                    break;
            }
        }
    }

    protected virtual void Init()
    {
        _anim = GetComponent<SPUM_Prefabs>()._anim;
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        switch (State)
        {
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Walk:
                UpdateWalk();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
            case Define.State.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() {}
    protected virtual void UpdateWalk() {}
    protected virtual void UpdateAttack() {}
    protected virtual void UpdateDead() {}
}
