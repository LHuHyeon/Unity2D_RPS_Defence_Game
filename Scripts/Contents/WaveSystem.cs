using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   WaveSystem.cs
 * Desc :   Wave를 관리
 *          다음 Wave 정보를 넘겨주고 Wave 시작을 맡는다.
 *
 & Functions
 &  [Public]
 &  : WaveStart()       - Wave 시작
 &  : NextWaveCheck()   - 다음 Wave 확인
 *
 */

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private int             _currentWaveIndex = 0;

    [SerializeField]
    private SpawningPool    _enemySpawner;

    private List<WaveData>  _waves;

    public void SetWave(List<WaveData> waves) { _waves = waves; }

    public void WaveStart()
    {
        // 다음 Wave 존재 확인
        if (_currentWaveIndex > _waves.Count)
        {
            Debug.Log("No Next Wave");
            return;
        }

        // 적 소환 시작
        _enemySpawner.StartWave(Managers.Game.CurrentWave);
    }

    public void NextWaveCheck()
    {
        // 다음 Wave 존재 확인
        if (_currentWaveIndex >= _waves.Count)
        {
            Debug.Log("No Next Wave");
            return;
        }

        // 다음 진행할 Wave 가져오기
        WaveData wave = _waves[_currentWaveIndex];

        // 보스 확인 (몬스터 최대 수가 1명이라면 보스 판정)
        if (wave.maxEnemyCount == 1)
            Managers.Game.IsBoss = true;

        Managers.Game.CurrentWave = wave;
        Managers.Game.GameScene.SetNextWave(wave);
        
        _currentWaveIndex++;
    }
}
