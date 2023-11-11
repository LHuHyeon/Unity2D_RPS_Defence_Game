using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File :   UI_DragSlot.cs
 * Desc :   마우스로 슬롯이 옮겨지는 과정을 보여주기 위한 슬롯
 *
 & Functions
 &  [Public]
 &  : DragSetImage()    - 드래그할 경우 이미지 활성화
 &  : SetColor()        - 색깔 설정
 *
 */

public class UI_DragSlot : MonoBehaviour
{
    public static UI_DragSlot   instance;

    public UI_ItemDragSlot      itemSlot;           // 슬롯
    public MercenaryTile        mercenaryTile;      // 용병 타일
    public Image                icon;               // 아이템 이미지
    public RectTransform        attackRangeMark;    // 공격 사거리 표시

    void Start()
    {
        instance = this;
        attackRangeMark.gameObject.SetActive(false);
    }

    // 용병 반환
    public MercenaryStat GetMercenary()
    {
        if ((itemSlot is UI_MercenarySlot) == true)
            return (itemSlot as UI_MercenarySlot)._mercenary;
        else if (mercenaryTile.IsFakeNull() == false)
            return mercenaryTile.GetMercenary().GetStat();

        return null;
    }

    // 드래그 할 경우 활성화
    public void DragSetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        SetColor(1);

        // 공격 사거리 표시
        float rangeSize = GetMercenary().AttackRange * 4f;
        attackRangeMark.gameObject.SetActive(true);
        attackRangeMark.localScale = new Vector3(rangeSize, rangeSize, 0f);
    }

    // 투명도 설정
    public void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }

    public void DragInfoClear()
    {
    }

    public void ClearSlot()
    {
        SetColor(0);
        attackRangeMark.gameObject.SetActive(false);

        mercenaryTile = null;
        itemSlot = null;

        Managers.Game.IsDrag = false;
    }
}
