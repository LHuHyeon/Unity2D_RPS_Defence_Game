using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private int     _evolutionPlanCount = 0;        // 진화 목표수
    private bool    _isEvolution        = false;    // 진화 가능 여부

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.EvolutionButton).gameObject.BindEvent(OnClickEvolutionButton);

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
        int     mercenaryCount  = Managers.Game.GameScene.GetMercenarySlot(_mercenary, false)?._itemCount ?? 0;
        
        _evolutionPlanCount     = ((int)_mercenary.CurrentEvolution + 1);

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = _evolutionPlanCount;

        // [슬롯에서 왔을 때] : 현재 용병이 진화가 안되어 있다면 -1 차감
        if (_slot.IsFakeNull() == false)
        {
            if (_mercenary.CurrentEvolution == Define.EvolutionType.Unknown)
                mercenaryCount--;
        }

        // [타일에서 왔을 때] : 현재 용병과 같은 필드의 용병 개수 가져오기
        if (_tile.IsFakeNull() == false)
            mercenaryCount += Managers.Game.GetMercenaryCount(_mercenary);

        // 현재 용병 수 / 필요 수
        GetText((int)Texts.EvolutionGaugeText).text = $"{mercenaryCount} / {_evolutionPlanCount}";

        // 진화 버튼 활성화/비활성화 투명도 설정
        if (mercenaryCount >= _evolutionPlanCount)
        {
            _isEvolution = true;
            SetColor(GetButton((int)Buttons.EvolutionButton).image, 1);
            SetColor(GetText((int)Texts.EvolutionButtonText), 1);
        }
        else
        {
            _isEvolution = false;
            SetColor(GetButton((int)Buttons.EvolutionButton).image, 0.5f);
            SetColor(GetText((int)Texts.EvolutionButtonText), 0.5f);
        }

        // 슬라이더 값 적용
        if (_mercenary.CurrentEvolution >= Define.EvolutionType.Star3)
        {
            evolutionSlider.value = evolutionSlider.maxValue;
            GetText((int)Texts.EvolutionButtonText).text = "Max";
            GetText((int)Texts.EvolutionGaugeText).text = "Max";
            _isEvolution = false;
        }
        else
        {
            evolutionSlider.value = mercenaryCount;
            GetText((int)Texts.EvolutionButtonText).text = "진화";
        }
    }

    private void OnClickEvolutionButton(PointerEventData eventData)
    {
        Debug.Log("OnClickEvolutionButton");

        if (_isEvolution == false)
            return;

        // 진화 재료로 사용될 slot 가져오기
        UI_MercenarySlot slot = Managers.Game.GameScene.GetMercenarySlot(_mercenary, false);

        // 슬롯에서 진화 시
        if (_slot.IsFakeNull() == false)
        {
            if (slot.IsFakeNull() == true)
                return;

            EvolutionSlot(slot);
            Managers.Game.GameScene.GetMercenarySlot(_mercenary, true)?.RefreshUI();
        }
        // 타일에서 진화 시
        else if (_tile.IsFakeNull() == false)
            EvolutionTile(slot);

        _infoPopup.RefreshUI();
    }

    // 슬롯에서 진화
    private void EvolutionSlot(UI_MercenarySlot slot)
    {
        // 진화 목표수 만큼 차감
        slot.SetCount(-_evolutionPlanCount);

        // 진화를 아직 안했다면 새 슬롯에서 진행
        if (_mercenary.CurrentEvolution == Define.EvolutionType.Unknown)
        {
            slot.SetCount(-1);
            _mercenary = Managers.Data.Mercenarys[_mercenary.Id].MercenaryClone<MercenaryStat>();
            _mercenary.CurrentEvolution++;

            Managers.Game.GameScene.MercenaryRegister(_mercenary, 1);

            _infoPopup._mercenary = _mercenary;
        }
        else
            _mercenary.CurrentEvolution++;
    }

    // 타일에서 진화
    private void EvolutionTile(UI_MercenarySlot slot)
    {
        // 슬롯 개수 먼저 차감 후 재료가 부족하면 필드 용병 차감
        int currentCount = _evolutionPlanCount;

        if (slot.IsFakeNull() == false)
        {
            currentCount -= slot._itemCount;
            slot.SetCount(-_evolutionPlanCount);
        }

        // 슬롯을 차감해도 재료가 부족하면 필드 용병 차감
        if (currentCount > 0)
        {
            List<GameObject> mercenarys = Managers.Game.GetMercenarys(_mercenary);
            for(int i=0; i<currentCount; i++)
            {
                if (mercenarys[i].IsFakeNull() == true)
                    return;
                    
                if (_tile._mercenary == mercenarys[i])
                {
                    currentCount++;
                    continue;
                }

                mercenarys[i].GetComponent<MercenaryController>()._tile.Clear();
                Managers.Game.Despawn(mercenarys[i]);
            }
        }

        // 재료가 충족 됐으니 진화 진행
        _mercenary.CurrentEvolution++;
        _mercenary.RefreshAddData();
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
