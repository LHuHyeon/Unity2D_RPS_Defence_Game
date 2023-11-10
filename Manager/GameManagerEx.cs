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
	public int Money;
	public int GameGold;

	public float PlayTime;

	// 현재 종족 레벨
	public int[] CurrentRaceLevel = new int[((int)Define.RaceType.MaxMercenary)] {0, 0, 0, 0};

	// 현재 추가된 종족 데미지
	public int[] RaceAddDamage = new int[((int)Define.RaceType.MaxMercenary)] {0, 0, 0, 0};

	// 현재 추가된 종족 데미지 %량
	public int[] RaceAddDamageParcent = new int[((int)Define.RaceType.MaxMercenary)] {0, 0, 0, 0};

	// 현재 추가된 직업 데미지 %량
	public int[] JobAddDamageParcent = new int[((int)Define.JobType.Max)] {0, 0, 0, 0};

	// 획득한 능력들
	public List<AbilityData> Abilities = new List<AbilityData>();

	public WaveData CurrentWave;
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

	#region 스탯

	public int[] CurrentRaceLevel
	{
		get { return _gameData.CurrentRaceLevel; }
		set { _gameData.CurrentRaceLevel = value; }
	}

	public int[] RaceAddDamage
	{
		get { return _gameData.RaceAddDamage; }
		set { _gameData.RaceAddDamage = value; }
	}

	public int[] RaceAddDamageParcent
	{
		get { return _gameData.RaceAddDamageParcent; }
		set { _gameData.RaceAddDamageParcent = value; }
	}

	public int[] JobAddDamageParcent
	{
		get { return _gameData.JobAddDamageParcent; }
		set { _gameData.JobAddDamageParcent = value; }
	}

	public List<AbilityData> Abilities
	{
		get { return _gameData.Abilities; }
		set { _gameData.Abilities = value; }
	}

	// 디버프별 값
	public Dictionary<Define.DeBuffType, int> DeBuffs = new Dictionary<DeBuffType, int>();

	// 현재 종족 강화 레벨
	public int GetRaceCurrentLevel(Define.RaceType raceType) { return CurrentRaceLevel[((int)raceType)]; }

	// 종족별 추가 데미지
	public int GetRaceAddDamage(Define.RaceType raceType) { return RaceAddDamage[((int)raceType)]; }

	// 종족별 추가 데미지 %
	public int GetRaceAddDamageParcent(Define.RaceType raceType) { return RaceAddDamageParcent[((int)raceType)]; }

	// 직업별 추가 데미지 %
	public int GetJobAddDamageParcent(Define.JobType jobType) { return JobAddDamageParcent[((int)jobType)]; }

	// 디버프 반환
	public float GetDebuff(Define.DeBuffType deBuffType)
	{
		if (DeBuffs.TryGetValue(deBuffType, out int value) == true)
			return value * 0.01f;

		return 0; 
	}

	// 확률 적으로 추가 골드
	public float 	GoldParcent { get; set; }
	public int 		AddGold { get; set; }

	#endregion

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

        // 특정 웨이브마다 능력 뽑기 진행
		if (Managers.Game.CurrentWave.waveLevel % 1 == 0)
		{
			Managers.UI.ShowPopupUI<UI_DrawAbilityPopup>().RefreshUI();
			return;
		}

		GameScene.OnRPSPopup();
	}

	// 종족별 데미지 강화
	public int RaceUpgradeDamage(Define.RaceType raceType, int upgradeDamage)
	{
		Managers.Game.RaceAddDamage[((int)raceType)] = upgradeDamage;
		return ++Managers.Game.CurrentRaceLevel[((int)raceType)];
	}

	// 능력 새로고침
	public void RefreshAbility()
	{
		RaceAddDamageParcent = new int[((int)Define.RaceType.MaxMercenary)] {0, 0, 0, 0};
		JobAddDamageParcent = new int[((int)Define.JobType.Max)] {0, 0, 0, 0};
		GoldParcent = 0;
		AddGold = 0;
		DeBuffs.Clear();

		for(int i=0; i<Abilities.Count; i++)
		{
			AbilityData abilityData = Abilities[i];

			switch (abilityData.abilityType)
			{
				// 직업별 공격력 강화 %
				case Define.AbilityType.WarriorDamageParcent:
				case Define.AbilityType.ArcherDamageParcent:
				case Define.AbilityType.WizardDamageParcent:
					JobAddDamageParcent[(int)abilityData.abilityType] += abilityData.currentValue;
					break;
				// 종족별 공격력 강화 %
				case Define.AbilityType.HumanDamageParcent:
				case Define.AbilityType.ElfDamageParcent:
				case Define.AbilityType.WereWolfDamageParcent:
					RaceAddDamageParcent[(int)abilityData.abilityType-3] += abilityData.currentValue;
					break;
				// 디버프 적용
				case Define.AbilityType.DefenceDecrease:	SetDebuff(Define.DeBuffType.DefenceDecrease, abilityData.currentValue); break;
				case Define.AbilityType.ShieldDecrease: 	SetDebuff(Define.DeBuffType.ShieldDecrease, abilityData.currentValue); 	break;
				case Define.AbilityType.SpeedDecerase: 		SetDebuff(Define.DeBuffType.Slow, abilityData.currentValue); 			break;
				// 골드 버프
				case Define.AbilityType.GoldParcent:
					GoldParcent += abilityData.currentValue;
					AddGold++;
					break;
			}
		}
	}

	// 디버프 추가
	private void SetDebuff(Define.DeBuffType deBuffType, int value)
	{
		if (DeBuffs.ContainsKey(deBuffType) == false)
			DeBuffs.Add(deBuffType, value);
		else
			DeBuffs[deBuffType] += value;
	}

	// 소환된 용병들 새로고침
	public void RefreshMercenary()
	{
		// 용병 정보 새로고침
		foreach(var mercenary in _mercenarys)
			mercenary.GetComponent<MercenaryController>().GetStat().RefreshAddData();
	}

	// stat과 같은 필드 용별들 객체 가져오기
	public List<GameObject> GetMercenarys(MercenaryStat stat)
	{
		List<GameObject> mercenarys = new List<GameObject>();

		// id 검사
		foreach(var mercenary in _mercenarys)
		{
			MercenaryStat mercenaryStat = mercenary.GetComponent<MercenaryController>().GetStat();
			if (mercenaryStat.IsSameMercenary(stat, false) == true)
				mercenarys.Add(mercenary);
		}

		return mercenarys;
	}

	// stat과 같은 필드 용병들 개수 가져오기
	public int GetMercenaryCount(MercenaryStat stat)
	{
		int count = 0;

		// id 검사
		foreach(var mercenary in _mercenarys)
		{
			MercenaryStat mercenaryStat = mercenary.GetComponent<MercenaryController>().GetStat();
			if (mercenaryStat.IsSameMercenary(stat, false) == true)
			{
				if (stat != mercenaryStat)
					count++;
			}
		}

		return count;
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
