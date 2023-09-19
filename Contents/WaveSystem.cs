using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int             currentWaveIndex = 0;

    [SerializeField]
    private float           waveTime = 20f;

    [SerializeField]
    private float           currentWaveTime = 0;

    [SerializeField]
    private SpawningPool    enemySpawner;

    void Start()
    {
        StartCoroutine(WaveCheckCoroutine());
    }

    // TODO : 현재는 자동 소환이지만 용병 뽑기가 생기면 public으로 호출
    // Next Wave Check
    private IEnumerator WaveCheckCoroutine()
    {
        // 데이터 존재 확인
        while (Managers.Data.IsData() == false)
            yield return null;

        // 다음 Wave 존재 확인
        currentWaveIndex++;
        if (Managers.Data.Waves.TryGetValue(currentWaveIndex, out WaveData waveData) == false)
        {
            Debug.Log("No Next Wave");
            yield break;
        }

        // 적 소환 시작
        enemySpawner.StartWave(waveData);

        StartCoroutine(WaveTimeCoroutine());
    }

    // Wave Time Check
    private IEnumerator WaveTimeCoroutine()
    {
        currentWaveTime = 0f;

        while (currentWaveTime < waveTime)
        {
            currentWaveTime += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(WaveCheckCoroutine());
    }
}
