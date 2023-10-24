using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        switch (_raceType)
        {
            case Define.RaceType.Human:
                Managers.Game.HumanAddDamage = _nextUpgradeData.humanDamage;
                _currentLevel = ++Managers.Game.CurrentHumanLevel;
                break;
            case Define.RaceType.Elf:
                Managers.Game.ElfAddDamage = _nextUpgradeData.elfDamage;
                _currentLevel = ++Managers.Game.CurrentElfLevel;
                break;
            case Define.RaceType.WereWolf:
                Managers.Game.WereWolfAddDamage = _nextUpgradeData.werewolfDamage;
                _currentLevel = ++Managers.Game.CurrentWereWolfLevel;
                break;
        }

        if (Managers.Data.Upgrades.TryGetValue(_currentLevel + 1, out _nextUpgradeData) == false)
            Debug.Log(_raceType.ToString() + " Max Level : " + _currentLevel);

        RefreshUI();
    }
}