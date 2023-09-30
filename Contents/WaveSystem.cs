using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int             _currentWaveIndex = 0;

    [SerializeField]
    private SpawningPool    _enemySpawner;

    public void WaveStart()
    {
        // 다음 Wave 존재 확인
        _currentWaveIndex++;
        if (Managers.Data.Waves.TryGetValue(_currentWaveIndex, out WaveData waveData) == false)
        {
            Debug.Log("No Next Wave");
            return;
        }

        Managers.Game.CurrentWave = waveData;

        // 적 소환 시작
        _enemySpawner.StartWave(waveData);
        Managers.Game.GameScene.SetNextWave(waveData);
    }
}
