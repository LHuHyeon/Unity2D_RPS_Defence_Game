using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class WaveData
{
    public int              waveLevel;          // 웨이브 레벨
    public Define.RaceType  race;               // 종족
    public int              hp;                 // 체력
    public int              defence;            // 방어력
    public float            moveSpeed;          // 이동 속도
    public int              gold;               // 골드
    public int              waveGold;           // 웨이브 성공 골드
    public int              maxEnemyCount;      // 적 생성 최대 횟수

    public float            spawnTime;          // 생성 주기

    public SpriteLibraryAsset   spriteLibrary;      // 캐릭터 파츠
}