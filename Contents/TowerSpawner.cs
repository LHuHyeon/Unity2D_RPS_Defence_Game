using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public bool SpawnTower(MercenaryStat stat, Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        // 타워 건설 여부 확인
        if (tile.IsBuildTower == true)
        {
            Debug.Log("Tile Build True!!");
            return false;
        }

        // 타워가 건설되어 있음으로 설정
        tile.IsBuildTower = true;

        // 선택한 타일의 위치에 타워 건설
        GameObject          go          = Managers.Game.Spawn(Define.WorldObject.Mercenary, "Mercenary/Mercenary", tileTransform);
        MercenaryController mercenary   = go.GetComponent<MercenaryController>();

        // 용병 정보 입력
        mercenary.SetStat(stat);
        mercenary.transform.position = tileTransform.position;
        mercenary.currentTile = tile;

        return true;
    }
}