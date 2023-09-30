using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    private float       waveTime = 20f;

    [SerializeField]
    private float       currentWaveTime = 0;

    [SerializeField]
    private Transform[] wayPoints;          // 이동 경로

    private WaveData    currentWave;

    public void StartWave(WaveData waveData)
    {
        currentWave = waveData;
        StartCoroutine(SpawnMonster());
    }

    private IEnumerator SpawnMonster()
    {
        int spawnEnemyCount = 0;

        StartCoroutine(WaveTimeCoroutine(false, 3));
        yield return new WaitForSeconds(3f);

        // TODO : 3초 카운트 다운 구현

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

        StartCoroutine(WaveTimeCoroutine(true, waveTime));
    }

    // Wave Time Check
    private IEnumerator WaveTimeCoroutine(bool isFormat, float time)
    {
        currentWaveTime = time;

        while (currentWaveTime >= 0f)
        {
            currentWaveTime -= Time.deltaTime;
            Managers.Game.GameScene.RefreshWaveTime(isFormat, currentWaveTime);

            yield return null;
        }
    }
}