﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;

/*
 * File :   DataManager.cs
 * Desc :   게임 시작 시 필요 데이터를 불러옵니다.
 */
 
public class DataManager : MonoBehaviour
{
    // 구글 스프레드 주소
    private const string URL = "https://docs.google.com/spreadsheets/d/1Td16WXEJ34lC1glVt7EnxKmdXnRYLFrD4DBTUrKZrVs/export?format=csv&gid=";

    public Dictionary<int, StartData>       Start       { get; private set; }
    public Dictionary<int, List<WaveData>>  Stages      { get; private set; }
    public Dictionary<int, MercenaryStat>   Mercenarys  { get; private set; }
    public Dictionary<int, BuffData>        Buff        { get; private set; }
    public Dictionary<int, UpgradeData>     Upgrades    { get; private set; }
    public Dictionary<int, TextData>        Texts       { get; private set; }

    public Dictionary<Define.AbilityType, AbilityData>  Abilities { get; private set; }

    // 등급마다 직업별로 용병들이 존재한 이중 딕셔너리
    public Dictionary<Define.GradeType, Dictionary<Define.JobType, List<MercenaryStat>>> JobByGrade { get; private set; }

	public void Init()
    {
        Buff = new Dictionary<int, BuffData>();

        StartCoroutine(DataRequest(StartRequest, Define.StartDataNumber));
        StartCoroutine(DataRequest(MercenaryRequest, Define.MercenaryDataNumber));
        StartCoroutine(DataRequest(OriginalBuffRequest, Define.OriginalBuffDataNumber));
        StartCoroutine(DataRequest(InstantBuffRequest, Define.InstantBuffDataNumber));
        StartCoroutine(DataRequest(UpgradeRequest, Define.UpgradeDataNumber));
        StartCoroutine(DataRequest(AbilityRequest, Define.AbilityDataNumber));
        StartCoroutine(StageWaveRequest());

        TextRequest();
    }

    public bool IsData()
    {
        if (Start.IsNull()      == true ||
            Mercenarys.IsNull() == true ||
            Upgrades.IsNull()   == true || 
            Buff.IsNull()       == true ||
            Abilities.IsNull()  == true ||
            isStageData         == false)
            return false;

        return true;
    }

    // 직업과 등급에 맞는 용병들 반환
    public List<MercenaryStat> GetMercenarys(Define.GradeType grade, Define.JobType job)
    {
        if (JobByGrade.TryGetValue(grade, out Dictionary<Define.JobType, List<MercenaryStat>> jobStat) == false)
        {
            Debug.Log($"JobByGrade Failed : {grade.ToString()} Grade");
            return null;
        }

        return jobStat[job];
    }

    // 구글 스프레드시트 가져오기
    private IEnumerator DataRequest(Action<string> onRequest, string dataNumber)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL+dataNumber);

        yield return www.SendWebRequest();

        string dataText = www.downloadHandler.text;
        
        onRequest.Invoke(dataText);
    }



#region 데이터 파싱

    #region Start Data

    private void StartRequest(string data)
    {
        Dictionary<int, StartData> dict = new Dictionary<int, StartData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            StartData startData = new StartData()
            {
                stageLevel = int.Parse(row[0]),
                waveTime = int.Parse(row[1]),
                maxWaveCount = int.Parse(row[2]),
                drawAbilityWave = int.Parse(row[3]),
                criticalDamage = int.Parse(row[4]),
            };

            dict.Add(startData.stageLevel, startData);
        }

        Start = dict;
    }

    #endregion

    #region Wave Data

    private int     currentStageIndex = 1;
    private bool    isStageData = false;
    private IEnumerator StageWaveRequest()
    {
        // stage 구글 시트 주소들
        string[] stageNumbers = new string[] {Define.Stage01DataNumber};

        Stages = new Dictionary<int, List<WaveData>>();

        for (int i=0; i<stageNumbers.Length; i++)
        {
            // 데이터 가져오기
            StartCoroutine(DataRequest(WaveRequest, stageNumbers[i]));
            
            while (currentStageIndex <= stageNumbers.Length)
            {
                // 해당 Stage의 Value가 들어올 때까지 기다리기
                if (Stages.ContainsKey(currentStageIndex) == true)
                    currentStageIndex++;

                yield return null;
            }
        }

        isStageData = true;
    }

    // 웨이브 마다 몬스터 스탯 저장 
    private void WaveRequest(string data)
    {
        List<WaveData> waves = new List<WaveData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            WaveData waveData = new WaveData()
            {
                waveLevel = int.Parse(row[0]),
                race = (Define.RaceType)int.Parse(row[1]),
                hp = int.Parse(row[2]),
                shield = int.Parse(row[3]),
                defence = int.Parse(row[4]),
                moveSpeed = float.Parse(row[5]),
                gold = int.Parse(row[6]),
                waveGold = int.Parse(row[7]),
                maxEnemyCount = int.Parse(row[8]),
                spawnTime = float.Parse(row[9]),
            };

            waves.Add(waveData);
        }

        // string Data 가져오기
        WaveStringData(waves);

        Stages.Add(currentStageIndex, waves);
    }

    private void WaveStringData(List<WaveData> waves)
    {
        string[] lines = Resources.Load<TextAsset>($"Data/Stage" + currentStageIndex).text.Split("\n");

        int currentIndex = 0;
        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            WaveData wave = waves[currentIndex];

            string  spriteLibPath   = $"UI/SpriteLibrary/Enemy/{wave.race.ToString()}/{row[1]}";
            wave.spriteLibrary      = Managers.Resource.Load<SpriteLibraryAsset>(spriteLibPath);

            currentIndex++;
        }
    }

    #endregion

    #region 용병 Data

    // 모든 용병 데이터 저장
    private void MercenaryRequest(string data)
    {
        Dictionary<int, MercenaryStat> dict = new Dictionary<int, MercenaryStat>();
        JobByGrade = new Dictionary<Define.GradeType, Dictionary<Define.JobType, List<MercenaryStat>>>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            MercenaryStat mercenaryStat = new MercenaryStat()
            {
                Id = int.Parse(row[0]),
                Grade = (Define.GradeType)int.Parse(row[1]),
                Race = (Define.RaceType)int.Parse(row[2]),
                Job = (Define.JobType)int.Parse(row[3]),
                SalePrice = int.Parse(row[4]),
                Damage = int.Parse(row[5]),
                AttackSpeed = float.Parse(row[6]),
                AttackRange = float.Parse(row[7]),
            };

            SetJobByGrade(mercenaryStat);
            dict.Add(mercenaryStat.Id, mercenaryStat);
        }

        // 진화 능력 가져오기
        StartCoroutine(DataRequest(EvolutionRequest, Define.EvolutionDataNumber));

        // string Data 가져오기
        MercenaryStringData(dict);

        Mercenarys = dict;
    }

    // 등급별로 직업을 나누고, 직업안에 List으로 용병 저장.
    private void SetJobByGrade(MercenaryStat mercenary)
    {
        // 등급 딕셔너리 공간 확인
        if (JobByGrade.TryGetValue(mercenary.Grade, out Dictionary<Define.JobType, List<MercenaryStat>> jobStat) == false)
        {
            jobStat = new Dictionary<Define.JobType, List<MercenaryStat>>();
            JobByGrade.Add(mercenary.Grade, jobStat);
        }

        // 직업 딕셔너리 공간 확인
        if (jobStat.TryGetValue(mercenary.Job, out List<MercenaryStat> mercenaryHash) == false)
        {
            List<MercenaryStat> mercenarys = new List<MercenaryStat>();
            mercenarys.Add(mercenary);
            jobStat.Add(mercenary.Job, mercenarys);

            return;
        }

        // 용병 넣기
        mercenaryHash.Add(mercenary);
    }

    private void MercenaryStringData(Dictionary<int, MercenaryStat> dict)
    {
        string[] lines = Resources.Load<TextAsset>($"Data/MercenaryData").text.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            int id = int.Parse(row[0]);

            MercenaryStat mercenaryStat = dict[id];

            mercenaryStat.SpriteLibrary  = Managers.Resource.Load<SpriteLibraryAsset>("UI/SpriteLibrary/Mercenary/"+row[1]);

            string projectilePath;

            if (mercenaryStat.Race == Define.RaceType.WereWolf)
                projectilePath = "Moon";
            else
            {
                switch (mercenaryStat.Job)
                {
                    case Define.JobType.Warrior:    projectilePath = "Slash";   break;
                    case Define.JobType.Archer:     projectilePath = "Arrow";   break;
                    case Define.JobType.Wizard:     projectilePath = "Sphere";  break;
                    default:                        projectilePath = "";        break;
                }
            }
            
            mercenaryStat.ProjectileIcon = Managers.Resource.Load<SpriteAtlas>("UI/Sprite/Projectile/" + projectilePath).GetSprite(row[2]);

            // 발사체 Prefab 가져오기
            mercenaryStat.Projectile = Managers.Resource.Load<GameObject>("Prefabs/Projectile/" + projectilePath);
            mercenaryStat.Icon = mercenaryStat.SpriteLibrary.GetSprite("Block", "0");

            dict[id] = mercenaryStat;
        }
    }

    #endregion

    #region 진화 Data

    private void EvolutionRequest(string data)
    {
        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            for(int i=1; i<=3; i++)
            {
                BuffData buff = Buff[int.Parse(row[i])];    // id에 맞는 버프 가져오기
                Mercenarys[y].Buffs.Add(buff);              // 추가
            }
        }
    }

    #endregion

    #region 버프 Data

    private void OriginalBuffRequest(string data)
    {
        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            OriginalBuffData buff = new OriginalBuffData()
            {
                id = int.Parse(row[0]),
                buffType = (Define.BuffType)int.Parse(row[1]),
                value = float.Parse(row[2]),
            };

            // 능력 설명
            switch (buff.buffType)
            {
                case Define.BuffType.Damage:        buff.descripition = $"공격력 {buff.value} 증가";   break;
                case Define.BuffType.DamageParcent: buff.descripition = $"공격력 {buff.value}% 증가";  break;
                case Define.BuffType.AttackSpeed:   buff.descripition = $"공격속도 {buff.value} 증가"; break;
                case Define.BuffType.AttackRange:   buff.descripition = $"공격범위 {buff.value} 증가"; break;
                case Define.BuffType.MultiShot:     buff.descripition = $"멀티샷 {buff.value} 증가";   break;
                case Define.BuffType.Splash:        buff.descripition = $"확산 피해 범위 {buff.value} 증가";   break;
            }

            Buff.Add(buff.id, buff);
        }
    }

    private void InstantBuffRequest(string data)
    {
        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            InstantBuffData buff = new InstantBuffData()
            {
                id = int.Parse(row[0]),
                isDeBuff = Convert.ToBoolean(int.Parse(row[1])),
                buffType = (Define.DeBuffType)int.Parse(row[2]),
                value = int.Parse(row[3]),
                parcentage = int.Parse(row[4]),
                time = float.Parse(row[5]),
            };

            buff.descripition = $"{buff.parcentage}% 확률로 {buff.time}초 동안 ";

            // 능력 설명
            switch (buff.buffType)
            {
                case Define.DeBuffType.DefenceDecrease:    buff.descripition += $"방어력 {buff.value}% 감소 부여";   break;
                case Define.DeBuffType.Slow:               buff.descripition += $"이동속도 {buff.value}% 감소 부여";  break;
                case Define.DeBuffType.Stun:               buff.descripition += $"기절 부여"; break;
            }

            Buff.Add(buff.id, buff);
        }
    }

    #endregion

    #region 강화 Data

    // 종족 강화 데이터
    private void UpgradeRequest(string data)
    {
        Dictionary<int, UpgradeData> dict = new Dictionary<int, UpgradeData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            UpgradeData upgradeData = new UpgradeData()
            {
                level = int.Parse(row[0]),
                prime = int.Parse(row[1]),
                raceDamage = new int[((int)Define.RaceType.MaxMercenary)] {0, int.Parse(row[2]), int.Parse(row[3]), int.Parse(row[4])},
            };

            dict.Add(upgradeData.level, upgradeData);
        }

        Upgrades = dict;
    }

    #endregion

    #region 능력 Data

    // 능력 데이터
    private void AbilityRequest(string data)
    {
        Dictionary<Define.AbilityType, AbilityData> dict = new Dictionary<Define.AbilityType, AbilityData>();

        string[] lines = data.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            Define.AbilityType abilityType = (Define.AbilityType)int.Parse(row[0]);

            AbilityData abilityData;
            if (dict.TryGetValue(abilityType, out abilityData) == false)
            {
                abilityData = new AbilityData() { abilityType = abilityType, values = new List<int>() };
                dict.Add(abilityType, abilityData);
            }

            abilityData.values.Add(int.Parse(row[1]));
        }

        AbilityStringData(dict);

        Abilities = dict;
    }

    private void AbilityStringData(Dictionary<Define.AbilityType, AbilityData> dict)
    {
        string[] lines = Resources.Load<TextAsset>($"Data/AbilityData").text.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            Define.AbilityType abilityType = (Define.AbilityType)int.Parse(row[0]);

            if (dict.TryGetValue(abilityType, out AbilityData abilityData) == false)
                Debug.Log(abilityType.ToString() + " Data Failed!!");

            abilityData.name = row[1];
            abilityData.descripition = row[2];
        }
    }

    #endregion

    #region 문자열 Data

    // 종족 강화 데이터
    private void TextRequest()
    {
        Dictionary<int, TextData> dict = new Dictionary<int, TextData>();

        string[] lines = Resources.Load<TextAsset>($"Data/TextData").text.Split("\n");

        for(int y = 1; y < lines.Length; y++)
        {
            string[] row = Row(lines[y]);

            if (row.IsNull() == true)
                continue;

            TextData textData = new TextData()
            {
                id = int.Parse(row[0]),
                kor = row[1],
            };

            dict.Add(textData.id, textData);
        }

        Texts = dict;
    }

    #endregion

#endregion

    // 가로 줄 읽기 (csv)
    private string[] Row(string line)
    {
        string[] row = line.Replace("\r", "").Split(',');

        if (row.Length == 0)
            return null;
        if (string.IsNullOrEmpty(row[0]))
            return null;

        return row;
    }
}
