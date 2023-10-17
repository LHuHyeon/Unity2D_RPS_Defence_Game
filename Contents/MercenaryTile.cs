using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MercenaryTile : MonoBehaviour
{
    public GameObject _mercenary;

    public MercenaryController GetMercenary() { return _mercenary.GetComponent<MercenaryController>(); }

    void Start()
    {
        SetEventHandler();
    }

    public void SetMercenary(GameObject go)
    {
        _mercenary = go;

        _mercenary.transform.SetParent(transform);
        _mercenary.transform.localPosition = Vector3.up * -0.35f;
    }

    public void Clear()
    {
        _mercenary = null;
    }

    // 타일 교체
    private void ChangeTile(MercenaryTile tile)
    {
        // 내 용병 임시 저장
        GameObject tempObj = _mercenary;

        // 여기에 용병 저장
        SetMercenary(tile._mercenary);

        // 내 용병을 상대 타일에 전달
        if (tempObj.IsFakeNull() == false)
            tile.SetMercenary(tempObj);
        else
            tile.Clear();
    }

    // 타일과 슬롯 교체
    private void ChangeTileAndSlot(UI_MercenarySlot slot)
    {
        // 용병이 존재 시 슬롯으로 이동 or 없으면 객체 생성
        if (_mercenary.IsFakeNull() == true)
            SetMercenary(Managers.Game.Spawn(Define.WorldObject.Mercenary, "Mercenary/Mercenary", transform));
        else
            Managers.Game.GameScene.MercenaryRegister(GetMercenary().GetStat());

        GetMercenary().SetStat(slot._mercenary);    // 스탯 설정
        slot.SetCount(-1);                          // 슬롯 -1 차감

        // 슬롯이 삭제되면 드래그 슬롯을 초기화 못함으로 여기서 해주기
        UI_DragSlot.instance.ClearSlot();
    }

    private void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (_mercenary.IsFakeNull() == true || Managers.Game.isDrag == true)
                return;

            Debug.Log("OnClickEvent");

            // 용병 정보 팝업 호출
            UI_MercenaryInfoPopup infoPopup = Managers.UI.FindPopup<UI_MercenaryInfoPopup>();
            if (infoPopup.IsFakeNull() == true)
                infoPopup = Managers.UI.ShowPopupUI<UI_MercenaryInfoPopup>();

            infoPopup.SetInfo(GetMercenary().GetStat());

        }, Define.UIEvent.Click);

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (Managers.Game.isDrag == true)
                return;
            
            Managers.Game.isDrag = true;

            if (_mercenary.IsFakeNull() == true)
                return;

            UI_DragSlot.instance.mercenaryTile = this;

            UI_DragSlot.instance.DragSetIcon(GetMercenary().GetStat().Icon);
            UI_DragSlot.instance.icon.transform.position = eventData.position;

        }, Define.UIEvent.BeginDrag);

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 마우스 드래그 방향으로 이동
            if (_mercenary.IsNull() == false && UI_DragSlot.instance.mercenaryTile.IsNull() == false)
                UI_DragSlot.instance.icon.transform.position = eventData.position;

        }, Define.UIEvent.Drag);
        
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_DragSlot.instance.ClearSlot();

        }, Define.UIEvent.EndDrag);

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_DragSlot dragSlot = UI_DragSlot.instance;

            // 드래그 슬롯에 용병 정보가 존재하는가?
            if (dragSlot.GetMercenary().IsNull() == true)
                return;

            // 내 자신일 경우
            if (dragSlot.mercenaryTile == this)
                return;

            // 타일 or 슬롯 어떤게 왔는지 확인 후 교체
            if (dragSlot.mercenaryTile.IsNull() == false)
                ChangeTile(dragSlot.mercenaryTile);
            else if (dragSlot.itemSlot.IsNull() == false)
                ChangeTileAndSlot(dragSlot.itemSlot as UI_MercenarySlot);

        }, Define.UIEvent.Drop);
    }
}
