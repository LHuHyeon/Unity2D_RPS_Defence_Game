﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * File :   SceneManagerEx.cs
 * Desc :   Scene Manager
 */

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        Debug.Log("Scene Load : " + type.ToString());

        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    public AsyncOperation LoadAsynScene(Define.Scene type)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Managers.Scene.GetSceneName(type));

        // Load가 끝나면 바로 불러올 지 선택
        operation.allowSceneActivation = false;

        return operation;
    }

    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
