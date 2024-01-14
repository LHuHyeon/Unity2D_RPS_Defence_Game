using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_ItemSlot.cs
 * Desc :   용병 Slot이 상속받는 Class
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : ClearSlot()   - 초기화
 &
 &  [Protected]
 &  : SetColor()        - 컬러 설정(투명도)
 &  : OnClickEvent()    - 슬롯 클릭 기능
 &  : SetEventHandler() - PointerEventData 설정
 *
 */
 
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
