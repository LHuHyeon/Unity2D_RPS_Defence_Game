using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   SpawningPool.cs
 * Desc :   Wave가 시작되면 몬스터를 생성 및 시간 계산
 *
 & Functions
 &  [Public]
 &  : StartWave()   - Wave 정보 적용
 &
 &  [private]
 &  : SpawnMonster()        - 몬스터 스폰 코루틴
 &  : WaveTimeCoroutine()   - Wave 시간 계산 코루틴
 *
 */

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    private float       _currentWaveTime = 0;

    [SerializeField]
    private Transform[] _wayPoints;          // 이동 경로

    private WaveData    _wave;

    public void StartWave(WaveData waveData)
    {
        _wave = waveData;
        Managers.Game.RemainEnemyCount = _wave.maxEnemyCount;

        StartCoroutine(SpawnMonster());
    }

    private IEnumerator SpawnMonster()
    {
        int spawnEnemyCount = 0;

        StartCoroutine(WaveTimeCoroutine(false, 3));
        yield return new WaitForSeconds(3f);

        Managers.Game.GameScene.RefreshWaveTime(true, Managers.Game.WaveTime);

        // 적 생성 최대치 만큼 생성
        while (spawnEnemyCount < _wave.maxEnemyCount)
        {
            // 몬스터 생성 후 컴포넌트 받기
            GameObject          clone   = Managers.Game.Spawn(Define.WorldObject.Enemy, "Enemy/Enemy");
            EnemyController     monster = clone.GetComponent<EnemyController>();

            // 만약 몬스터 최대 수가 1이라면 보스로 인식하고 크기 키워주기
            if (_wave.maxEnemyCount == 1)
                monster._isBoss = true;

            // 몬스터의 이동 경로 세팅
            monster.SetWayPoint(_wayPoints);

            // Wave 정보 부여
            monster.SetWave(_wave);

            spawnEnemyCount++;

            yield return new WaitForSeconds(_wave.spawnTime);
        }

        StartCoroutine(WaveTimeCoroutine(true, Managers.Game.WaveTime));
    }

    // Wave Time Check
    private IEnumerator WaveTimeCoroutine(bool isFormat, float time)
    {
        _currentWaveTime = time;

        // 남은 시간이 있다면 반복
        while (_currentWaveTime >= 0f)
        {
            // 몬스터가 다 처치되면 보상 지급
            if (Managers.Game.RemainEnemyCount == 0)
            {
                Managers.Game.WaveReward();
                Managers.Game.GameScene.RefreshWaveTime(false, 0);
                yield break;
            }

            // 시간 계산
            _currentWaveTime -= Time.deltaTime;
            Managers.Game.GameScene.RefreshWaveTime(isFormat, _currentWaveTime);

            yield return null;
        }
    }
}