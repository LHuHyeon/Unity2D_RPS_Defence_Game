using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobyScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Loby;

        Debug.Log("LobyScene Init");

        return true;
    }

    protected override void SetScene()
    {
        Managers.UI.ShowSceneUI<UI_LobyScene>();
    }
}
