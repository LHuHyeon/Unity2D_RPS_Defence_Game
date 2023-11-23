using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
