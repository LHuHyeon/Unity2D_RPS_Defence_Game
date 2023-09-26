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

    public UI_ItemSlot          itemSlot;       // 슬롯
    public MercenaryController  mercenary;      // 용병
    public Image                icon;           // 아이템 이미지

    void Start()
    {
        instance = this;
    }

    // 용병 반환
    public MercenaryStat GetMercenary()
    {
        if ((itemSlot is UI_MercenaryItem) == true)
            return (itemSlot as UI_MercenaryItem)._mercenary;
        else if (mercenary.IsNull() == false)
            return mercenary.GetMercenaryStat();

        return null;
    }

    // 드래그 할 경우 활성화
    public void DragSetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        SetColor(1);
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
        itemSlot = null;
        mercenary = null;
    }

    public void ClearSlot()
    {
        SetColor(0);
    }
}
