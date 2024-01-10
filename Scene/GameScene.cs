using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * File :   GameScene.cs
 * Desc :   Game Scene이 시작될 때 가장먼저 호출
 *
 & Functions
 &  [Protected]
 &  : Init()		- 초기 설정
 &	: OnScene()		- Scene 기능 실행
 &
 &  [Private]
 &  : StageSizeSetting()	- Stage 크기 설정
 *
 */

public class GameScene : BaseScene
{
	// TODO : 게임을 로비에서 시작한다면 그 곳에서 결정하기
	private int currentStage = 1;

    protected override bool Init()
    {
		if (base.Init() == false)
			return false;

        SceneType = Define.Scene.Game;

		Debug.Log("GameScene Init");
		return true;
	}

	protected override void OnScene()
	{
		// 시작 기본 데이터 적용
		StartData startData = Managers.Data.Start[currentStage];
		startData.SetGameData();

		// 게임을 진행할 Prefab 생성
		Managers.Game.GameScene = Managers.UI.ShowSceneUI<UI_GameScene>();
		Managers.Game.WaveSystem = Managers.Resource.Instantiate("Stage/Stage" + startData.stageLevel).GetComponent<WaveSystem>();
		
		Managers.Game.WaveSystem.SetWave(Managers.Data.Stages[currentStage]);

		StageSizeSetting();
	}

	// 스테이지 크기 설정
	private void StageSizeSetting()
	{
		SpriteRenderer sr = Managers.Game.WaveSystem.GetComponent<SpriteRenderer>();

		float spriteX = sr.sprite.bounds.size.x;
		float spriteY = sr.sprite.bounds.size.y;

		float screenY = Camera.main.orthographicSize * 2;
		float screenX = screenY / Screen.height * Screen.width;

		sr.transform.localScale = new Vector2(Mathf.Ceil(screenX / spriteX), Mathf.Ceil(screenY / spriteY));
	}
}