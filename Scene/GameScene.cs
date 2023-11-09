using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
		if (base.Init() == false)
			return false;

        SceneType = Define.Scene.Game;

		StartCoroutine(OnSceneCoroutine());

		Debug.Log("GameScene Init");
		return true;
	}

	private IEnumerator OnSceneCoroutine()
	{
		while(Managers.Data.IsData() == false)
			yield return null;

		// Managers.UI.ShowPopupUI<UI_DrawAbilityPopup>().RefreshUI();
		Managers.Game.GameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
		Managers.Game.WaveSystem = Managers.Resource.Instantiate("Stage/Stage01").GetComponent<WaveSystem>();
	}
}
