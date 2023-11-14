using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int             _currentWaveIndex = 0;

    [SerializeField]
    private SpawningPool    _enemySpawner;

    private List<WaveData>  _waves;

    public void SetWave(List<WaveData> waves)
    {
        _waves = waves;
    }

    public void WaveStart()
    {
        // 다음 Wave 존재 확인
        if (_currentWaveIndex >= _waves.Count)
        {
            Debug.Log("No Next Wave");
            return;
        }

        // 다음 진행할 Wave 가져오기
        WaveData wave = _waves[_currentWaveIndex];

        Managers.Game.CurrentWave = wave;

        // 적 소환 시작
        _enemySpawner.StartWave(wave);
        Managers.Game.GameScene.SetNextWave(wave);
        
        _currentWaveIndex++;
    }
}
