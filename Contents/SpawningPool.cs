using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[TODO]
현재는 테스트로 하이라이커에서 직접 몬스터를 받지만
몬스터 종류가 많아지고 Wave를 진행할 수 있다면 코드를 수정한다.
*/

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    private GameObject  monsterPrefab;      // 몬스터 프리펩
    [SerializeField]
    private float       spawnTime;          // 생성 주기
    [SerializeField]
    private Transform[] wayPoints;          // 이동 경로

    void Awake()
    {
        StartCoroutine(SpawnMonster());
    }

    private IEnumerator SpawnMonster()
    {
        while (true)
        {
            // 몬스터 생성 후 컴포넌트 받기
            GameObject          clone   = Instantiate(monsterPrefab);
            EnemyController     monster = clone.GetComponent<EnemyController>();

            // 몬스터의 이동 경로 세팅
            monster.SetUp(wayPoints);

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
