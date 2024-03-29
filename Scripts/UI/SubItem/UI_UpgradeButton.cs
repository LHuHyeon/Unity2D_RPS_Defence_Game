using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File :   UI_UpgradeButton.cs
 * Desc :   "UI_GameScene"의 하위 항목으로 사용
 *          용병의 각 종족들의 공격력을 강화하는데 사용
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : SetInfo()     - 정보 설정
 &  : RefreshUI()   - UI 새로고침
 &
 &  [Private]
 &  : OnClickUpgradeButton()    - 강화 버튼
 *
 */
 
public class UI_UpgradeButton : UI_Base
{   
    enum Images
    {
        UpgradeIcon,
    }

    enum Texts
    {
        UpgradeNameText,
        UpgradeLevelText,
        UpgradeGoldText,
    }

    public Define.RaceType  _raceType;
    
    private int             _currentLevel = 0;
    private UpgradeData     _nextUpgradeData;

    private Image           _background;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetImage((int)Images.UpgradeIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Upgrade_"+_raceType.ToString());
        GetText((int)Texts.UpgradeNameText).text = _raceType.ToString();

        gameObject.BindEvent(OnClickUpgradeButton);
        _background = GetComponent<Image>();

        Managers.Game.GameScene._onRefreshGoldAction -= RefreshUI;
        Managers.Game.GameScene._onRefreshGoldAction += RefreshUI;

        RefreshUI();

        return true;
    }

    public void SetInfo(Define.RaceType raceType)
    {
        _raceType           = raceType;
        _nextUpgradeData    = Managers.Data.Upgrades[_currentLevel + 1];

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (Managers.Game.GameGold >= _nextUpgradeData.prime)
            _background.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Btn_Green");
        else
            _background.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Btn_Dark");

        GetText((int)Texts.UpgradeLevelText).text   = "Lv. " + _currentLevel;
        GetText((int)Texts.UpgradeGoldText).text    = $@"<color=yellow>G {_nextUpgradeData.prime}</color>";
    }

    private void OnClickUpgradeButton(PointerEventData eventData)
    {
        if (Managers.Game.GameGold < _nextUpgradeData.prime)
            return;

        Managers.Game.GameGold -= _nextUpgradeData.prime;

        _currentLevel = Managers.Game.RaceUpgradeDamage(_raceType, _nextUpgradeData.raceDamage[((int)_raceType)]);

        if (Managers.Data.Upgrades.TryGetValue(_currentLevel + 1, out _nextUpgradeData) == false)
            Debug.Log(_raceType.ToString() + " Max Level : " + _currentLevel);

        RefreshUI();

        Managers.UI.FindPopup<UI_MercenaryInfoPopup>()?.RefreshUI();
    }
}