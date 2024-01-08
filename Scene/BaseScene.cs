using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        StartCoroutine(OnSceneCoroutine());

        return true;
    }

    protected virtual void SetScene() {}

    public virtual void Clear() { }

    // 기본 데이터를 다 가져오면 Scene 코드 진행
    private IEnumerator OnSceneCoroutine()
    {
		while(Managers.Data.IsData() == false)
			yield return null;

        SetScene();
    }
}
