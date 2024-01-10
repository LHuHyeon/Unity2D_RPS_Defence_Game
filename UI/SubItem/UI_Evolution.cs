using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_Evolution.cs
 * Desc :   "UI_MercenaryInfoPopup"의 하위 항목으로 사용
 *          용병의 진화 정보를 확인하고 [진화 버튼]을 클릭하여 진화할 수 있다.
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : SetInfo()             - 정보 설정
 &  : RefreshEvolution()    - 진화 정보 UI 새로고침
 &
 &  [Private]
 &  : OnClickEvolutionButton()  - 진화 버튼
 &  : EvolutionSlot()           - Slot에서 진화 진행
 &  : EvolutionTile()           - Tile에서 진화 진행
 &  : SubSlotMercenary()        - 진화 시 Slot에 있는 용병 차감
 &  : SubTileMercenary()        - 진화 시 Tile에 있는 용병 차감
 &  : SetColor()                - 컬러 설정(투명도)
 *
 */

public class UI_Evolution : UI_Base
{
    enum GameObjects
    {
        EvolutionGauge,
    }

    enum Buttons
    {
        EvolutionButton,
    }

    enum Texts
    {
        EvolutionGaugeText,
        EvolutionButtonText,
    }

    public  UI_MercenaryInfoPopup   _infoPopup;

    private MercenaryTile           _tile;
    private UI_MercenarySlot        _slot;
    private MercenaryStat           _mercenary;

    private int     _requiredCount  = 1;        // 진화 필요 수
    private bool    _isEvolution    = false;    // 진화 가능 여부

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.EvolutionButton).gameObject.BindEvent(OnClickEvolutionButton);

        Slider  evolutionSlider = GetObject((int)GameObjects.EvolutionGauge).GetComponent<Slider>();
        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = _requiredCount;

        RefreshEvolution();

        return true;
    }

    public void SetInfo()
    {
        _tile       = _infoPopup._tile;
        _slot       = _infoPopup._slot;
        _mercenary  = _infoPopup._mercenary;

        RefreshEvolution();
    }

    public void RefreshEvolution()
    {
        if (_init == false)
            return;

        Slider  evolutionSlider = GetObject((int)GameObjects.EvolutionGauge).GetComponent<Slider>();
        int     mercenaryCount  = Managers.Game.GetMercenaryCount(_mercenary);

        // [슬롯에서 왔을 때] : 슬롯은 본인 포함이기 때문에 재료 개수 -1 차감
        if (_slot.IsFakeNull() == false)
            mercenaryCount += _slot._itemCount - 1;

        // [타일에서 왔을 때] : 용병 정보와 같은 슬롯 가져오기
        if (_tile.IsFakeNull() == false)
            mercenaryCount += Managers.Game.GameScene.GetMercenarySlot(_mercenary)?._itemCount ?? 0;

        // 현재 용병 수 / 필요 수
        GetText((int)Texts.EvolutionGaugeText).text = $"{mercenaryCount} / {_requiredCount}";

        _isEvolution = mercenaryCount >= _requiredCount;
        evolutionSlider.value = mercenaryCount;

        // 진화 버튼 활성화/비활성화 투명도 설정
        float alpha = _isEvolution == true ? 1f : 0.5f;
        SetColor(GetButton((int)Buttons.EvolutionButton).image, alpha);
        SetColor(GetText((int)Texts.EvolutionButtonText), alpha);

        // 진화 최대치라면 Max 표시
        GetText((int)Texts.EvolutionButtonText).text = _mercenary.CurrentEvolution >= Define.EvolutionType.Star3 ? "Max" : "진화";
    }

    private void OnClickEvolutionButton(PointerEventData eventData)
    {
        Debug.Log("OnClickEvolutionButton");

        if (_isEvolution == false)
            return;

        // 진화 최대치 확인
        if (_mercenary.CurrentEvolution >= Define.EvolutionType.Star3)
            return;

        // 재료 차감 진행 (슬롯 -> 타일 순서로 차감)
        if (SubSlotMercenary() == false)
            SubTileMercenary();

        // 진화 진행
        if (_slot.IsFakeNull() == false)        EvolutionSlot();
        else if (_tile.IsFakeNull() == false)   EvolutionTile();
    }

    // 슬롯에서 진화
    private void EvolutionSlot()
    {
        // 진화를 시켜준 후 새 슬롯에 추가
        Define.EvolutionType evolution = _mercenary.CurrentEvolution;
        _mercenary = Managers.Data.Mercenarys[_mercenary.Id].MercenaryClone<MercenaryStat>();
        _mercenary.CurrentEvolution = evolution;
        _mercenary.CurrentEvolution++;

        _slot.SetCount(-1);
        _slot = Managers.Game.GameScene.MercenaryRegister(_mercenary, 1);
        _infoPopup.SetInfoSlot(_slot);
    }

    // 타일에서 진화
    private void EvolutionTile()
    {
        _mercenary.CurrentEvolution++;
        _mercenary.RefreshAddData();
        
        _infoPopup.SetInfoTile(_tile);
    }

    // 슬롯 용병 차감
    private bool SubSlotMercenary()
    {
        // 진화할 슬롯이 개수가 1개라면
        if (_slot.IsFakeNull() == false)
        {
            if (_slot._itemCount == 1)
                return false;
        }
        
        // 진화 재료로 사용될 slot 가져오기
        UI_MercenarySlot slot = Managers.Game.GameScene.GetMercenarySlot(_mercenary);
        
        // slot이 존재하면 재료 차감
        if (slot.IsFakeNull() == false)
        {
            slot.SetCount(-_requiredCount);
            return true;
        }

        return false;
    }

    // 필드 용병 차감
    private void SubTileMercenary()
    {
        // 진화 재료로 사용될 필드 용병 가져오기
        List<GameObject> mercenarys = Managers.Game.GetMercenarys(_mercenary);

        bool isTile = _tile.IsFakeNull() == false;

        for(int i=0; i<mercenarys.Count; i++)
        {
            if (mercenarys[i].IsFakeNull() == true)
                continue;

            if (isTile == true)        
            {
                // 타일의 같은 용병인지 확인
                if (_mercenary._mercenary.gameObject == mercenarys[i])
                    continue;
            }

            // 필드의 용병 삭제
            Managers.Game.Despawn(mercenarys[i]);

            return;
        }

        return;
    }

    private void SetColor(TextMeshProUGUI text, float alpha)    { text.color = SetColor(text.color, alpha); }
    private void SetColor(Image icon, float alpha)              { icon.color = SetColor(icon.color, alpha); }

    // 투명도 설정
    private Color SetColor(Color color, float alpha)
    {
        Color _color = color;
        _color.a = alpha;
        return _color;
    }
}
