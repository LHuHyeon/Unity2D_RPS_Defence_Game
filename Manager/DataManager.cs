using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D.Animation;

public class DataManager : MonoBehaviour
{
    // 구글 스프레드 주소
    private const string URL = "https://docs.google.com/spreadsheets/d/1Td16WXEJ34lC1glVt7EnxKmdXnRYLFrD4DBTUrKZrVs/export?format=csv&gid=";

    public Dictionary<int, WaveData>                        Waves       { get; private set; }
    public Dictionary<int, MercenaryStat>                   Mercenarys  { get; private set; }
    public Dictionary<Define.GradeType, List<GameObject>>   Grades      { get; private set; }

	public void Init()
    {
        StartCoroutine(DataRequest(WaveRequest, Define.WaveDataNumber));
    }

    public bool IsData()
    {
        if (Waves.IsNull() == true)
            return false;

        return true;
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
                moveSpeed = int.Parse(row[4]),
                gold = int.Parse(row[5]),
                waveGold = int.Parse(row[6]),
                maxEnemyCount = int.Parse(row[7]),
                spawnTime = float.Parse(row[8]),
                spriteLibrary = Managers.Resource.Load<SpriteLibraryAsset>("UI/SpriteLibrary/"+row[9]),
            };

            dict.Add(waveData.waveLevel, waveData);
        }

        Waves = dict;
    }

    private void MercenaryRequest(string data)
    {
        Dictionary<int, MercenaryStat> dict = new Dictionary<int, MercenaryStat>();

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
                SpriteLibrary = Managers.Resource.Load<SpriteLibraryAsset>("UI/SpriteLibrary/"+row[7]),
                Projectile = Managers.Resource.Load<GameObject>("Prefabs/Projectile/"+row[8]),
            };

            dict.Add(mercenaryStat.Id, mercenaryStat);
        }

        Mercenarys = dict;
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
