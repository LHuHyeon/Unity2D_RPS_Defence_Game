using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Load;

        Managers.UI.ShowSceneUI<UI_LoadScene>();

        Debug.Log("LoadScene Init");

        return true;
    }
}
