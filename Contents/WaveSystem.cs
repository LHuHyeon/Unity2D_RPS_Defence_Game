using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int             currentWaveIndex = 0;

    [SerializeField]
    private SpawningPool    enemySpawner;

    public void WaveStart()
    {
        // 다음 Wave 존재 확인
        currentWaveIndex++;
        if (Managers.Data.Waves.TryGetValue(currentWaveIndex, out WaveData waveData) == false)
        {
            Debug.Log("No Next Wave");
            return;
        }

        // 적 소환 시작
        enemySpawner.StartWave(waveData);
        Managers.Game.GameScene.SetNextWave(waveData);
    }
}
