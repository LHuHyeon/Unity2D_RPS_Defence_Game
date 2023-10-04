using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public bool SpawnTower(MercenaryStat stat, Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        // 용병 존재 여부 확인
        if (tile.mercenaryObj.IsFakeNull() == false)
        {
            Debug.Log("Tile Build True!!");
            return false;
        }

        // 선택한 타일의 위치에 타워 건설
        GameObject          go          = Managers.Game.Spawn(Define.WorldObject.Mercenary, "Mercenary/Mercenary", tileTransform);
        MercenaryController mercenary   = go.GetComponent<MercenaryController>();
        
        // 타워 설정
        tile.mercenaryObj = go;

        // 용병 정보 입력
        mercenary.SetStat(stat);
        mercenary.transform.localPosition = Vector3.up * -0.45f;
        mercenary.currentTile = tile;

        return true;
    }
}