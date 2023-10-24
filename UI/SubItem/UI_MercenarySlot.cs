using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MercenarySlot : UI_ItemDragSlot
{
    enum GameObjects
    {
        StarGrid,
    }

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

    public MercenaryStat        _mercenary;

    private bool                _isScroll = false;
    private List<GameObject>    _starIcons = new List<GameObject>();
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
        {
            _starIcons.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        
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

        // 별 모두 비활성화
        for(int i=0; i<_starIcons.Count; i++)
            _starIcons[i].SetActive(false);

        // 진화한 만큼 별 활성화
        for(int i=0; i<((int)_mercenary.CurrentEvolution); i++)
            _starIcons[i].SetActive(true);

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
        UI_MercenaryInfoPopup infoPopup = Managers.UI.FindPopup<UI_MercenaryInfoPopup>() ?? Managers.UI.ShowPopupUI<UI_MercenaryInfoPopup>();
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
            _isScroll = true;

        if (_isScroll == true)
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

            Managers.UI.FindPopup<UI_MercenaryInfoPopup>()?.Clear();
        }
    }

    protected override void OnDragEvent(PointerEventData eventData)
    {
        // 스크롤 중이라면
        if (_isScroll == true)
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

        if (_isScroll == true)
        {
            Managers.Game.GameScene._mercenaryTabScroll.OnEndDrag(eventData);
            _isScroll = false;
        }
    }

    protected override void OnDropEvent(PointerEventData eventData)
    {
        UI_DragSlot dragSlot = UI_DragSlot.instance;

        // 타일 정보가 존재 하는가?
        if (dragSlot.mercenaryTile.IsFakeNull() == true)
            return;

        // 현재 슬롯의 용병 정보와 같은지
        if (_mercenary.IsSameMercenary(dragSlot.GetMercenary()))
            SetColor(1);
        else
            Managers.Game.GameScene.MercenaryRegister(dragSlot.GetMercenary());

        // 타일 초기화
        Managers.Game.Despawn(dragSlot.mercenaryTile._mercenary);
        dragSlot.mercenaryTile.Clear();
    }

#endregion
}
