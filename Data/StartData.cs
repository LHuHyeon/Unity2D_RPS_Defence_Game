using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   StartData.cs
 * Desc :   게임 시작 Data
 *
 & Functions
 &  [Public]
 &  : SetGameData() - 게임 데이터 설정
 *
 */
 
public class StartData
{
    public int stageLevel;
    public int waveTime;
    public int maxWaveCount;
    public int drawAbilityWave;
    public int criticalDamage;

    public void SetGameData()
    {
        GameManagerEx game = Managers.Game;

        game.WaveTime = this.waveTime;
        game.MaxWaveCount = this.maxWaveCount;
		game.DrawAbilityWave = this.drawAbilityWave;
        game.CriticalDamageParcent = this.criticalDamage;
    }
}
