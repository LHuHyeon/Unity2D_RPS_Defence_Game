using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

/*
 * File :   BaseController.cs
 * Desc :   캐릭터의 기본 기능
 *
 & Functions
 &  [Protected]
 &  : Init()            - 초기 설정
 &  : UpdateIdle()      - 멈춤일 때 Update
 &  : UpdateWalk()      - 움직일 때 Update
 &  : UpdateAttack()    - 공격할 때 Update
 &  : DeadCoroutine()   - 죽었을 때 Coroutine
 *
 */

public abstract class BaseController : MonoBehaviour
{
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    protected bool              isInit  = false;    // 초기 설정 여부

    protected Coroutine         coDead;             // 죽었을 때 코루틴 실행 확인용

    protected Animator          _anim;
    protected SpriteLibrary     _spriteLibrary;     // 캐릭터 파츠

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
                    _anim.CrossFade("Walk", 0.1f, -1, 0);
                    break;
                case Define.State.Attack:
                    _anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case Define.State.Dead:
                    if (coDead.IsNull() == true)
                        _anim.CrossFade("Dead", 0.1f, -1, 0);
                    break;
            }
        }
    }

    protected virtual void Init()
    {
        _anim = GetComponent<Animator>();
        _spriteLibrary = Utils.FindChild<SpriteLibrary>(this.gameObject);
    }

    void Awake()
    {
        Init();
        isInit = true;
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
            {
                if (coDead.IsNull() == true)
                    coDead = StartCoroutine(DeadCoroutine());
            }
                break;
        }
    }

    protected virtual void UpdateIdle() {}
    protected virtual void UpdateWalk() {}
    protected virtual void UpdateAttack() {}
    protected virtual IEnumerator DeadCoroutine() { yield return null; }
}
