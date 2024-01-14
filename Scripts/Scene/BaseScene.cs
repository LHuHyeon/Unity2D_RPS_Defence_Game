using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   BaseScene.cs
 * Desc :   Scene이 시작되고 가장 먼저 호출되는 Class
 *
 & Functions
 &  [Public]
 &  : Clear()   - 초기화
 &
 &  [Protected]
 &  : Init()    - 초기 설정
 &  : OnScene() - 씬 기능 실행
 &
 &  [Private]
 &  : OnSceneCoroutine()    - 필요 데이터 확인 후 Scene 코드 진행
 *
 */

public class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType = Define.Scene.Unknown;

    protected bool _init = false;

    private void Start()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_init == true)
            return false;

        _init = true;

        // EvenySystem 생성
        GameObject go = GameObject.Find("EventSystem");
        if (go == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        // 카메라 기능 추가
        GameObject camera = Camera.main.gameObject;
        camera.GetOrAddComponent<Physics2DRaycaster>();
        camera.GetOrAddComponent<CameraResolution>();

        return true;
    }

    public virtual void Clear() { }
}
