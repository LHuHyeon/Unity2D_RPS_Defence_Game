using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   LobyScene.cs
 * Desc :   Loby Scene이 시작될 때 가장먼저 호출
 *
 & Functions
 &  [Protected]
 &  : Init()		- 초기 설정
 &	: OnScene()		- Scene 기능 실행
 *
 */

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

    protected override void OnScene()
    {
        Managers.UI.ShowSceneUI<UI_LobyScene>();
    }
}
