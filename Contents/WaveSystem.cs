using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private SpawningPool    enemySpawner;
    private int             currentWaveIndex = 0;

    // TODO : 웨이브 게임 적용시키기
    public void StartWave()
    {
        currentWaveIndex++;

        // 다음 Wave가 존재한다면
        if (Managers.Data.Wave.TryGetValue(currentWaveIndex, out WaveData waveData) == true)
        {
            enemySpawner.StartWave(waveData);
        }
    }
}
