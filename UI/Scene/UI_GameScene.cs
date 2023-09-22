using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        MercenaryTab,
        MercenaryContent,
        UIDetector,
    }

    enum PlayTab
	{
		None,
		Mercenary,
		Upgrade,
		Mix,
	}

    private List<UI_MercenaryItem> _MercenaryItems = new List<UI_MercenaryItem>();
    private PlayTab _tab = PlayTab.None;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

		Managers.Resource.Instantiate("UI/SubItem/UI_DragSlot", transform);
        
        PopulateMercenary();

        SetEventHandler();

        return true;
    }

    // 용병 슬롯 등록
    public void MercenaryRegister(MercenaryStat mercenaryStat, int count = 1)
    {
        // 현재 존재하는 용병 슬롯 탐지
        foreach(UI_MercenaryItem slot in _MercenaryItems)
        {
            if (slot._mercenary == mercenaryStat)
            {
                slot.SetCount(count);
                return;
            }
        }

        // 중복된 용병 슬롯이 없으면 생성하여 저장
        UI_MercenaryItem item = Managers.UI.MakeSubItem<UI_MercenaryItem>(GetObject((int)GameObjects.MercenaryContent).transform);
        item.SetInfo(mercenaryStat);

        _MercenaryItems.Add(item);
    }

    public void RefreshUI()
    {

    }

    // 용병 채우기 (TODO : Test 용)
    private void PopulateMercenary()
    {
        var mercenaryParent = GetObject((int)GameObjects.MercenaryContent);

        foreach(Transform child in mercenaryParent.transform)
            Managers.Resource.Destroy(child.gameObject);
        
        for(int i=1; i<=3; i++)
        {
            UI_MercenaryItem item = Managers.UI.MakeSubItem<UI_MercenaryItem>(mercenaryParent.transform);

            item.SetInfo(Managers.Data.Mercenarys[i]);

            _MercenaryItems.Add(item);
        }
    }

    private void SetEventHandler()
    {
        // 2D Object를 위한 UI 탐지 Event 설정
        GameObject go = GetObject((int)GameObjects.UIDetector);

        go.BindEvent((PointerEventData eventData)=>
        {
            UI_DragSlot.instance.DragSetIcon(UI_DragSlot.instance.mercenaryStat.Icon);
            UI_DragSlot.instance.icon.transform.position = eventData.position;

        }, Define.UIEvent.BeginDrag);

        go.BindEvent((PointerEventData eventData)=>
        {
            // 마우스 드래그 방향으로 아이템 이동
            if (UI_DragSlot.instance.GetMercenary().IsNull() == false)
                UI_DragSlot.instance.icon.transform.position = eventData.position;

        }, Define.UIEvent.Drag);

        go.BindEvent((PointerEventData eventData)=>
        {
            // ui가 아닌 곳에 놓으면 드래그 초기화
            if (EventSystem.current.IsPointerOverGameObject() == false)
                UI_DragSlot.instance.ClearSlot();

            Managers.Game.isDrag = false;

        }, Define.UIEvent.EndDrag);

        // 용병 슬롯 탭 Drop 설정
        GetObject((int)GameObjects.MercenaryTab).BindEvent((PointerEventData eventData)=>
        {
            // 드래그 정보가 존재하는가?
            if (UI_DragSlot.instance.GetMercenary().IsNull() == true)
                return;

            // 이미 등록된 슬롯인가?
            if (IsSlotCheck(UI_DragSlot.instance.itemSlot as UI_MercenaryItem) == true)
            {
                UI_DragSlot.instance.ClearSlot();
                return;
            }

            // 용병이 소환되어 있다면 삭제
            if (UI_DragSlot.instance.GetMercenary().Mercenary.IsFakeNull() == false)
                Managers.Resource.Destroy(UI_DragSlot.instance.GetMercenary().Mercenary);

            MercenaryRegister(UI_DragSlot.instance.GetMercenary());
            UI_DragSlot.instance.mercenartObj = null;
            UI_DragSlot.instance.ClearSlot();
        }, Define.UIEvent.Drop);
    }

    public bool IsSlotCheck(UI_MercenaryItem mercenaryItem)
    {
        if (mercenaryItem.IsFakeNull() == true)
            return false;

        foreach(UI_MercenaryItem slot in _MercenaryItems)
        {
            if (slot == mercenaryItem)
                return true;
        }

        return false;
    }
}
