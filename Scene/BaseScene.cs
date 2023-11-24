﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (_init)
            return false;

        _init = true;
        GameObject go = GameObject.Find("EventSystem");
        if (go == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

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
