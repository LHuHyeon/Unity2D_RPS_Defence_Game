using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   LoginScene.cs
 * Desc :   Login Scene이 시작될 때 가장먼저 호출
 *
 & Functions
 &  [Protected]
 &  : Init()		- 초기 설정
 &	: OnScene()		- Scene 기능 실행
 *
 */

public class LoginScene : BaseScene
{
    protected override bool Init()
    {
		if (base.Init() == false)
			return false;

        SceneType = Define.Scene.Login;
		
		Managers.UI.ShowSceneUI<UI_LoginScene>();

		Debug.Log("LoginScene Init");
		
		return true;
	}
}
