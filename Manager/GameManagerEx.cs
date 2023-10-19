using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;

// TODO : 유저 정보는 서버에서 관리
// 게임 진행 시 저장할 데이터
[Serializable]
public class GameData
{
	public string Name;

	public int Money;
	public int GameGold;

	public float PlayTime;

	// 현재 종족 레벨
	public int CurrentHumanLevel = 0;
	public int CurrentElfLevel = 0;
	public int CurrentWereWolfLevel = 0;

	// 현재 추가된 종족 데미지
	public int HumanAddDamage = 0;
	public int ElfAddDamage = 0;
	public int WereWolfAddDamage = 0;

	public WaveData CurrentWave;

	/*
	필수 데이터
	1. 용병 컬렉션
	*/
}

public class GameManagerEx
{
	private GameData _gameData = new GameData();
	public 	GameData SaveData { get { return _gameData; } set { _gameData = value; } }

	public UI_GameScene GameScene  	{ get; set; }
	public WaveSystem	WaveSystem	{ get; set; }

	public bool isDrag = false;
	public int 	remainEnemys = 0;

	private HashSet<GameObject> _mercenarys = new HashSet<GameObject>();
	private HashSet<GameObject> _enemys = new HashSet<GameObject>();

	#region 스탯
	public string Name
	{
		get { return _gameData.Name; }
		set { _gameData.Name = value; }
	}

	#endregion

	#region 재화
	public int Money
	{
		get { return _gameData.Money; }
		set { _gameData.Money = value; }
	}
	
	public int GameGold
	{
		get { return _gameData.GameGold; }
		set 
		{
			int addGold = value - _gameData.GameGold;
			_gameData.GameGold = value;

			GameScene.RefreshGold(addGold);
		}
	}

	#endregion

	#region 시간

	public float PlayTime
	{
		get { return _gameData.PlayTime; }
		set { _gameData.PlayTime = value; }
	}

	public float waveRemainingTime;		// Wave 남은 시간

	#endregion

	public int CurrentHumanLevel
	{
		get { return _gameData.CurrentHumanLevel; }
		set { _gameData.CurrentHumanLevel = value; }
	}

	public int CurrentElfLevel
	{
		get { return _gameData.CurrentElfLevel; }
		set { _gameData.CurrentElfLevel = value; }
	}

	public int CurrentWereWolfLevel
	{
		get { return _gameData.CurrentWereWolfLevel; }
		set { _gameData.CurrentWereWolfLevel = value; }
	}

	public int HumanAddDamage
	{
		get { return _gameData.HumanAddDamage; }
		set { _gameData.HumanAddDamage = value; }
	}

	public int ElfAddDamage
	{
		get { return _gameData.ElfAddDamage; }
		set { _gameData.ElfAddDamage = value; }
	}

	public int WereWolfAddDamage
	{
		get { return _gameData.WereWolfAddDamage; }
		set { _gameData.WereWolfAddDamage = value; }
	}

	public WaveData CurrentWave
	{
		get { return _gameData.CurrentWave; }
		set { _gameData.CurrentWave = value; }
	}

	public void Init()
	{
	}

	// 웨이브 보상
	public void WaveReward()
	{
		GameGold += CurrentWave.waveGold;

		GameScene.OnRPSPopup();
	}

	public int GetRaceAddDamage(Define.RaceType raceType)
	{
		switch (raceType)
		{
			case Define.RaceType.Human:
				return HumanAddDamage;
			case Define.RaceType.Elf:
				return ElfAddDamage;
			case Define.RaceType.WereWolf:
				return WereWolfAddDamage;
			default:
				return 0;
		}
	}

    // 캐릭터 소환
	public Action<int> OnEnemySpawnEvent;
	public Action<int> OnMercenarySpawnEvent;
	public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
		GameObject go = Managers.Resource.Instantiate(path, parent);

		return CharacterSpawn(type, go);
    }

    public GameObject Spawn(Define.WorldObject type, GameObject obj, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(obj, parent);

        return CharacterSpawn(type, go);
    }

	private GameObject CharacterSpawn(Define.WorldObject type, GameObject go)
	{
        switch(type)
        {
            case Define.WorldObject.Enemy:
				{
					_enemys.Add(go);
					if (OnEnemySpawnEvent.IsNull() == false)
						OnEnemySpawnEvent.Invoke(1);
				}
                break;
            case Define.WorldObject.Mercenary:
				{
					_mercenarys.Add(go);
					if (OnMercenarySpawnEvent.IsNull() == false)
						OnMercenarySpawnEvent.Invoke(1);
				}
                break;
            default:
                Debug.Log("GameManager : Null Type");
                break;
        }

        return go;
	}

    // 객체 타입 확인
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc.IsNull() == true)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    // 캐릭터 삭제
    public void Despawn(GameObject go)
    {
        switch(GetWorldObjectType(go))
        {
            case Define.WorldObject.Enemy:
                {
                    if (_enemys.Contains(go))
                    { 
                        _enemys.Remove(go);
                        if (OnEnemySpawnEvent.IsNull() == false)
                            OnEnemySpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Mercenary:
                {
                    if (_mercenarys.Contains(go))
                    {
                        _mercenarys.Remove(go);
                        if (OnMercenarySpawnEvent.IsNull() == false)
                            OnMercenarySpawnEvent.Invoke(-1);
                    }
                }
                break;
        }

        Managers.Resource.Destroy(go);
    }

	#region Save & Load	
	public string _path = Application.persistentDataPath + "/SaveData.json";

	public void SaveGame()
	{
		string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
		File.WriteAllText(_path, jsonStr);
		Debug.Log($"Save Game Completed : {_path}");
	}

	public bool LoadGame()
	{
		if (File.Exists(_path) == false)
			return false;

		string fileStr = File.ReadAllText(_path);
		GameData data = JsonUtility.FromJson<GameData>(fileStr);
		if (data != null)
		{
			Managers.Game.SaveData = data;
		}

		Debug.Log($"Save Game Loaded : {_path}");
		return true;
	}
	#endregion
}
