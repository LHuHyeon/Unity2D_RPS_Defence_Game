using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
