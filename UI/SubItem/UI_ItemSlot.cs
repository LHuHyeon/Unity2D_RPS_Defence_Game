using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_Base
{
    protected Image icon;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetEventHandler();

        return true;
    }

    protected virtual void OnClickEvent(PointerEventData eventData){}
    protected virtual void OnBeginDragEvent(PointerEventData eventData){}
    protected virtual void OnDragEvent(PointerEventData eventData){}
    protected virtual void OnEndDragEvent(PointerEventData eventData){}
    protected virtual void OnDropEvent(PointerEventData eventData){}

    private void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>{ OnClickEvent(eventData); },        Define.UIEvent.Click);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnBeginDragEvent(eventData); },    Define.UIEvent.BeginDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDragEvent(eventData); },         Define.UIEvent.Drag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEndDragEvent(eventData); },      Define.UIEvent.EndDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDropEvent(eventData); },         Define.UIEvent.Drop);
    }

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }
}
