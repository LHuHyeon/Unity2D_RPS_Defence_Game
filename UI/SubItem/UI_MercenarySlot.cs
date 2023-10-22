using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MercenarySlot : UI_ItemDragSlot
{
    enum Images
    {
        Background,
        Icon,
        JobLabel,
        JobLabelIcon,
    }

    enum Texts
    {
        ItemCountText,
    }

    public MercenaryStat    _mercenary;
    private bool            isScroll = false;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        _icon = GetImage((int)Images.Icon);

        RefreshUI();

        return true;
    }

    public void SetInfo(MercenaryStat mercenary, int count = 1)
    {
        _mercenary = mercenary;

        SetCount(count);

        RefreshUI();
    }

    public override void RefreshUI()
    {
        if (_init == false)
            return;

        if (_mercenary.IsNull() == true)
        {
            SetColor(0);
            return;
        }

        SetColor(255);

        _icon.sprite = _mercenary.Icon;
        GetText((int)Texts.ItemCountText).text = _itemCount.ToString();
        GetImage((int)Images.Background).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_Grade_"+_mercenary.Grade.ToString());
        GetImage((int)Images.JobLabel).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_JobIcon_"+_mercenary.Job.ToString());
        GetImage((int)Images.JobLabelIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Job_"+_mercenary.Job.ToString());
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        _mercenary = null;
        GetImage((int)Images.Icon).sprite = null;

        Managers.Game.GameScene.RemoveMercenarySlot(this);

        base.ClearSlot();
    }

#region EventHandler

    protected override void OnClickEvent(PointerEventData eventData)
    {
        if (Managers.Game.isDrag == true)
            return;

        Debug.Log("OnClickEvent");

        // 용병 정보 팝업 호출
        UI_MercenaryInfoPopup infoPopup = Managers.UI.FindPopup<UI_MercenaryInfoPopup>();
        if (infoPopup.IsFakeNull() == true)
            infoPopup = Managers.UI.ShowPopupUI<UI_MercenaryInfoPopup>();

        infoPopup.SetInfoSlot(this);
    }

    protected override void OnBeginDragEvent(PointerEventData eventData)
    {
        if (_mercenary.IsNull() == true || Managers.Game.isDrag == true)
            return;

        Managers.Game.isDrag = true;

        // 마우스 드래그 방향 확인
        Vector2 dir = eventData.delta.normalized;

        // 왼쪽, 오른쪽으로 움직이면 탭 스크롤 조작
        if (dir == Vector2.left || dir == Vector2.right)
            isScroll = true;

        if (isScroll == true)
        {
            // 스크롤 시작
            Managers.Game.GameScene._mercenaryTabScroll.OnBeginDrag(eventData);
        }
        else
        {
            // 슬롯 드래그 시작
            UI_DragSlot.instance.itemSlot = this;

            UI_DragSlot.instance.DragSetIcon(_icon.sprite);
            UI_DragSlot.instance.icon.transform.position = eventData.position;
        }
    }

    protected override void OnDragEvent(PointerEventData eventData)
    {
        // 스크롤 중이라면
        if (isScroll == true)
        {
            Managers.Game.GameScene._mercenaryTabScroll.OnDrag(eventData);
            return;
        }

        // 슬롯을 마우스 드래그 방향으로 아이템 이동
        if (_mercenary.IsNull() == false && UI_DragSlot.instance.itemSlot.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnEndDragEvent(PointerEventData eventData)
    {
        UI_DragSlot.instance.ClearSlot();

        if (isScroll == true)
        {
            Managers.Game.GameScene._mercenaryTabScroll.OnEndDrag(eventData);
            isScroll = false;
        }
    }

    protected override void OnDropEvent(PointerEventData eventData)
    {
        UI_DragSlot dragSlot = UI_DragSlot.instance;

        // 타일 정보가 존재 하는가?
        if (dragSlot.mercenaryTile.IsFakeNull() == true)
            return;

        // 내 용병과 다르면 다른 슬롯에 추가 or 같으면 여기서 개수 추가
        if (dragSlot.GetMercenary().Id != _mercenary.Id)
            Managers.Game.GameScene.MercenaryRegister(dragSlot.GetMercenary());
        else
            SetCount(1);

        // 타일 초기화
        Managers.Game.Despawn(dragSlot.mercenaryTile._mercenary);
        dragSlot.mercenaryTile.Clear();
    }

#endregion
}
