using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[TODO]
현재는 테스트로 하이라이커에서 직접 몬스터를 받지만
몬스터 종류가 많아지고 Wave를 진행할 수 있다면 코드를 수정한다.
*/

public class SpawningPool : MonoBehaviour
{
    private GameObject  monsterPrefab;      // 몬스터 프리펩
    private float       spawnTime;          // 생성 주기

    private WaveData    currentWave;

    [SerializeField]
    private Transform[] wayPoints;          // 이동 경로

    public void StartWave(WaveData waveData)
    {
        currentWave = waveData;
        StartCoroutine(SpawnMonster());
    }

    private IEnumerator SpawnMonster()
    {
        int spawnEnemyCount = 0;

        // 적 생성 최대치 만큼 생성
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // 몬스터 생성 후 컴포넌트 받기
            GameObject          clone   = Managers.Game.Spawn(Define.WorldObject.Enemy, monsterPrefab);
            EnemyController     monster = clone.GetComponent<EnemyController>();

            // Wave 정보 부여
            monster.SetWave(currentWave);

            // 몬스터의 이동 경로 세팅
            monster.SetWayPoint(wayPoints);

            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }
}