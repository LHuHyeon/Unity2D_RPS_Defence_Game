using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    public bool SpawnTower(Transform tileTransform)
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
        mercenary.SetStat(UI_DragSlot.instance.GetMercenary());
        mercenary.transform.position = tileTransform.position;
        mercenary.currentTile = tile;

        // 들고 있는 용병이 슬롯에서 온거면
        if ((UI_DragSlot.instance.itemSlot is UI_MercenaryItem) == true)
        {
            UI_MercenaryItem mercenaryItem = UI_DragSlot.instance.itemSlot as UI_MercenaryItem;

            // 슬롯 개수 차감
            mercenaryItem.SetCount(-1);
        }

        // 드래그 슬롯 초기화
        UI_DragSlot.instance.ClearSlot();

        return true;
    }
}