using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScene : BaseScene
{
	// TODO : 게임을 로비에서 시작한다면 그 곳에서 결정하기
	private int currentStage = 1;

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

		// 시작 기본 데이터 적용
		StartData startData = Managers.Data.Start[currentStage];
		startData.SetGameData();

		// 게임을 진행할 Prefab 생성
		Managers.Game.GameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
		Managers.Game.WaveSystem = Managers.Resource.Instantiate("Stage/Stage" + startData.stageLevel).GetComponent<WaveSystem>();
	}
}
