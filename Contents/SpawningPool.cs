using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPool : MonoBehaviour
{
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
            GameObject          clone   = Managers.Game.Spawn(Define.WorldObject.Enemy, "Enemy/Enemy");
            EnemyController     monster = clone.GetComponent<EnemyController>();

            // 몬스터의 이동 경로 세팅
            monster.SetWayPoint(wayPoints);

            // Wave 정보 부여
            monster.SetWave(currentWave);

            spawnEnemyCount++;

            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }
}