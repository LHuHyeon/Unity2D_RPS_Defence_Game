using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Wave가 시작되면 몬스터를 생성한다.
* Wave 시간은 여기서 계산한다.
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

        while (_currentWaveTime >= 0f)
        {
            if (Managers.Game.RemainEnemyCount == 0)
            {
                Managers.Game.WaveReward();
                Managers.Game.GameScene.RefreshWaveTime(false, 0);
                yield break;
            }

            _currentWaveTime -= Time.deltaTime;
            Managers.Game.GameScene.RefreshWaveTime(isFormat, _currentWaveTime);

            yield return null;
        }
    }
}