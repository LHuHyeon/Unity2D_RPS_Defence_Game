using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetEventHandler();

        return true;
    }

    protected void OnClickEvnet()
    {

    }

    protected void OnBeginDragEvnet()
    {

    }

    protected void OnDragEvnet()
    {

    }

    protected void OnEndDragEvnet()
    {

    }

    private void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            OnClickEvnet();
        }, Define.UIEvent.Click);

        
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            OnBeginDragEvnet();
        }, Define.UIEvent.BeginDrag);
        
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            OnDragEvnet();
        }, Define.UIEvent.Drag);
        
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            OnEndDragEvnet();
        }, Define.UIEvent.EndDrag);
    }
}
