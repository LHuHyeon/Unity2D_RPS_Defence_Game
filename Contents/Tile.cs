using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public GameObject       mercenaryObj;

    public MercenaryController GetMercenary()
    {
        return mercenaryObj.GetComponent<MercenaryController>();
    }

    // 용병 설정
    public void SetMercenary(GameObject go)
    {
        mercenaryObj = go;

        mercenaryObj.transform.SetParent(transform);
        mercenaryObj.transform.localPosition = Vector3.up * -0.45f;
    }

    // 타일 정보 교체
    public void TileChange(Tile tile)
    {
        // 내 용병 임시 저장
        GameObject tempObj = mercenaryObj;

        // 여기에 용병 저장
        SetMercenary(tile.mercenaryObj);

        // 내 용병을 상대 타일에 전달
        if (tempObj.IsFakeNull() == false)
            tile.SetMercenary(tempObj);
        else
            tile.Clear();
    }

    public void Clear()
    {
        mercenaryObj = null;
    }
}
