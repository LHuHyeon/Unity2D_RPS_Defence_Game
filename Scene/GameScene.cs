using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
TODO : Stage 맵 생성
*/

public class GameScene : BaseScene
{
    protected override bool Init()
    {
		if (base.Init() == false)
			return false;

        SceneType = Define.Scene.Game;
		// Managers.UI.ShowPopupUI<UI_TitlePopup>();
		Debug.Log("GameScene Init");
		return true;
	}
}
