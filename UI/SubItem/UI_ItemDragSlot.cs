using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File :   UI_ItemDragSlot.cs
 * Desc :   드래그드랍에 사용되는 Slot이 상속받는 Class
 *
 & Functions
 &  [Public]
 &  : RefreshUI()   - UI 새로고침
 &  : SetCount()    - 개수 설정
 &
 &  [Protected]
 &  : OnBeginDragEvent()    - 드래그 시작 기능
 &  : OnDragEvent()         - 드래그 중 기능
 &  : OnEndDragEvent()      - 드래그 끝 기능
 &  : OnDropEvent()         - 드래그 드랍 기능
 &  : SetEventHandler()     - PointerEventData 설정
 *
 */
 
public class UI_ItemDragSlot : UI_ItemSlot
{
    public int _itemCount;

    public virtual void RefreshUI() {}

    public virtual void SetCount(int count = 1)
    {
        _itemCount += count;

        RefreshUI();

        // 용병 슬롯들 정렬
        Managers.Game.GameScene?.SortMercenarySlot();
        
        // 개수가 없다면
        if (_itemCount <= 0)
            ClearSlot();
    }

    protected virtual void OnBeginDragEvent(PointerEventData eventData){}
    protected virtual void OnDragEvent(PointerEventData eventData){}
    protected virtual void OnEndDragEvent(PointerEventData eventData){}
    protected virtual void OnDropEvent(PointerEventData eventData){}

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        gameObject.BindEvent((PointerEventData eventData)=>{ OnBeginDragEvent(eventData); },    Define.UIEvent.BeginDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDragEvent(eventData); },         Define.UIEvent.Drag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEndDragEvent(eventData); },      Define.UIEvent.EndDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDropEvent(eventData); },         Define.UIEvent.Drop);
    }
}
