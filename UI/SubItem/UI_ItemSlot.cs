using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : UI_Base
{
    protected Image _icon;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetEventHandler();

        return true;
    }

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float _alpha)
    {
        Color color = _icon.color;
        color.a = _alpha;
        _icon.color = color;
    }
    
    protected virtual void OnClickEvent(PointerEventData eventData){}

    protected virtual void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>{ OnClickEvent(eventData); },    Define.UIEvent.Click);
    }

    public virtual void ClearSlot()
    {
        SetColor(0);

        Managers.Resource.Destroy(this.gameObject);
    }
}
