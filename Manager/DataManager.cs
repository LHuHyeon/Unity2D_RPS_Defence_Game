﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D.Animation;

public class DataManager : MonoBehaviour
{
    // 구글 스프레드 주소
    private const string URL = "https://docs.google.com/spreadsheets/d/1Td16WXEJ34lC1glVt7EnxKmdXnRYLFrD4DBTUrKZrVs/export?format=csv&gid=";

    public Dictionary<int, WaveData>        Waves       { get; private set; }
    public Dictionary<int, MercenaryStat>   Mercenarys  { get; private set; }

    // 등급마다 직업별로 용병들이 존재한 이중 딕셔너리
    public Dictionary<Define.GradeType, Dictionary<Define.JobType, HashSet<MercenaryStat>>> JobByGrade { get; private set; }

	public void Init()
    {
        StartCoroutine(DataRequest(WaveRequest, Define.WaveDataNumber));
        StartCoroutine(DataRequest(MercenaryRequest, Define.MercenaryDataNumber));
    }

    public bool IsData()
    {
        if (Waves.IsNull()      == true  ||
            Mercenarys.IsNull() == true     )
            return false;

        return true;
    }

    // 직업과 등급에 맞는 용병들 반환
    public HashSet<MercenaryStat> GetMercenarys(Define.GradeType grade, Define.JobType job)
    {
        if (JobByGrade.TryGetValue(grade, out Dictionary<Define.JobType, HashSet<MercenaryStat>> jobStat) == false)
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

    // 웨이브 마다 몬스터 스탯 저장 
    private void WaveRequest(string data)
    {
        Dictionary<int, WaveData> dict = new Dictionary<int, WaveData>();

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
                defence = int.Parse(row[3]),
                moveSpeed = float.Parse(row[4]),
                gold = int.Parse(row[5]),
                waveGold = int.Parse(row[6]),
                maxEnemyCount = int.Parse(row[7]),
                spawnTime = float.Parse(row[8]),
                spriteLibrary = Managers.Resource.Load<SpriteLibraryAsset>("UI/SpriteLibrary/Enemy/"+row[9]),
            };

            dict.Add(waveData.waveLevel, waveData);
        }

        Waves = dict;
    }

    // 모든 용병 데이터 저장
    private void MercenaryRequest(string data)
    {
        Dictionary<int, MercenaryStat> dict = new Dictionary<int, MercenaryStat>();
        JobByGrade = new Dictionary<Define.GradeType, Dictionary<Define.JobType, HashSet<MercenaryStat>>>();

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
                Damage = int.Parse(row[4]),
                AttackRate = float.Parse(row[5]),
                AttackRange = float.Parse(row[6]),
                SpriteLibrary = Managers.Resource.Load<SpriteLibraryAsset>("UI/SpriteLibrary/Mercenary/"+row[7]),
                AnimatorController = Managers.Resource.Load<RuntimeAnimatorController>("Animator/"+row[8]),
                Projectile = Managers.Resource.Load<GameObject>("Prefabs/Projectile/"+row[9]),
            };

            mercenaryStat.Icon = mercenaryStat.SpriteLibrary.GetSprite("Block", "0");

            SetJobByGrade(mercenaryStat);
            dict.Add(mercenaryStat.Id, mercenaryStat);
        }

        Mercenarys = dict;
    }

    // 등급별로 직업을 나누고, 직업안에 HashSet으로 용병 저장.
    private void SetJobByGrade(MercenaryStat mercenary)
    {
        // 등급 딕셔너리 공간 확인
        if (JobByGrade.TryGetValue(mercenary.Grade, out Dictionary<Define.JobType, HashSet<MercenaryStat>> jobStat) == false)
        {
            jobStat = new Dictionary<Define.JobType, HashSet<MercenaryStat>>();
            JobByGrade.Add(mercenary.Grade, jobStat);
        }

        // 직업 딕셔너리 공간 확인
        if (jobStat.TryGetValue(mercenary.Job, out HashSet<MercenaryStat> mercenaryHash) == false)
        {
            HashSet<MercenaryStat> mercenarys = new HashSet<MercenaryStat>();
            mercenarys.Add(mercenary);
            jobStat.Add(mercenary.Job, mercenarys);

            return;
        }

        // 용병 넣기
        mercenaryHash.Add(mercenary);
    }

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

#endregion
}
