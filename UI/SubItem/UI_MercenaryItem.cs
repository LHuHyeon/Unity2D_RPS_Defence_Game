using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MercenaryItem : UI_ItemSlot
{
    enum Images
    {
        Background,
        Icon,
    }

    enum Texts
    {
        ItemCountText,
    }

    public MercenaryStat    _mercenary;
    public int              _itemCount;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        icon = GetImage((int)Images.Icon);

        RefreshUI();

        return true;
    }

    public void SetInfo(MercenaryStat mercenary, int count = 1)
    {
        _mercenary = mercenary;

        SetCount(count);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (_mercenary.IsNull() == true)
        {
            SetColor(0);
            return;
        }

        SetColor(255);

        icon.sprite = _mercenary.Icon;
        GetText((int)Texts.ItemCountText).text = _itemCount.ToString();
    }

    protected override void OnClickEvent(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.itemSlot.IsFakeNull() == false)
            return;

        Debug.Log("OnClickEvent");
        // TODO : 정보 or 소환 확인
    }

    protected override void OnBeginDragEvent(PointerEventData eventData)
    {
        if (_mercenary.IsNull() == true)
            return;

        Managers.Game.isDrag = true;

        UI_DragSlot.instance.itemSlot = this;

        UI_DragSlot.instance.DragSetIcon(icon.sprite);
        UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnDragEvent(PointerEventData eventData)
    {
        // 마우스 드래그 방향으로 아이템 이동
        if (_mercenary.IsNull() == false && UI_DragSlot.instance.itemSlot.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnEndDragEvent(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
            UI_DragSlot.instance.ClearSlot();

        Managers.Game.isDrag = false;
    }

    protected override void OnDropEvent(PointerEventData eventData)
    {
        // 드래그 슬롯에 용병 정보가 존재하는가?
        if (UI_DragSlot.instance.GetMercenary().IsNull() == true)
            return;

        // 내 자신일 경우
        if (UI_DragSlot.instance.itemSlot == this)
        {
            UI_DragSlot.instance.ClearSlot();
            return;
        }

        // 내 용병과 다르다면
        if (UI_DragSlot.instance.GetMercenary() != _mercenary)
        {
            // 용병이 소환되어 있다면 삭제
            if (UI_DragSlot.instance.GetMercenary().Mercenary.IsFakeNull() == false)
                Managers.Resource.Destroy(UI_DragSlot.instance.GetMercenary().Mercenary);

            // 다른 슬롯에 등록
            if (Managers.Game.GameScene.IsSlotCheck(UI_DragSlot.instance.itemSlot as UI_MercenaryItem) == false)
                Managers.Game.GameScene.MercenaryRegister(UI_DragSlot.instance.GetMercenary());

            UI_DragSlot.instance.mercenartObj = null;
            UI_DragSlot.instance.ClearSlot();
            return;
        }

        // 나랑 같은 용병이기 떄문에 여기서 카운터 증가
        SetColor(1);

        // 드래그 초기화
        UI_DragSlot.instance.ClearSlot();
    }

    public virtual void SetCount(int count = 1)
    {
        _itemCount += count;

        RefreshUI();
        
        // 개수가 없다면
        if (_itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public virtual void ClearSlot()
    {
        _mercenary = null;
        GetImage((int)Images.Icon).sprite = null;

        SetColor(0);

        Managers.Resource.Destroy(this.gameObject);
    }
}
