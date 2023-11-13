using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartData
{
    public int stageLevel;
    public int waveTime;
    public int drawAbilityWave;
    public int criticalDamage;

    public void SetGameData()
    {
        GameManagerEx game = Managers.Game;

        game.WaveTime = this.waveTime;
		game.DrawAbilityWave = this.drawAbilityWave;
        game.CriticalDamageParcent = this.criticalDamage;
    }
}
