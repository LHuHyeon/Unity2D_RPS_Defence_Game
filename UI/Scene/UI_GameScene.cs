using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DamageNumbersPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        MercenaryTab,
        UpgradeTab,
        MercenaryContent,
        StatusGold,
        MercenaryFocus,
        UpgradeFocus,
        UpgradeGrid,
    }

    enum Sliders
    {
        EnemySlider,
    }

    enum Buttons
    {
        PauseButton,
        GameSpeedButton,
        MercenaryButton,
        UpgradeButton,
        CompositionButton,
        StartButton,
        AbilityListButton,
        TestRegistarButton,
    }
    
    enum Texts
    {
        WaveTimeText,
        EnemyHpText,
        EnemyShieldText,
        EnemyDefenceText,
        WaveLevelText,
        GoldText,
        EnemyCountText,
        GameSpeedButtonText,
        MercenaryText,
        UpgradeText,
    }

    public enum PlayTab
	{
		None,
		Mercenary,      // 용병
		Upgrade,        // 강화
		Composition,    // 조합 구성
	}

    public ScrollRect       _mercenaryTabScroll;

    private List<UI_MercenarySlot> _mercenarySlots = new List<UI_MercenarySlot>();
    
    private GameManagerEx   _game;
    private WaveData        _wave;

    private PlayTab         _tab = PlayTab.None;    // 현재 탭

    private float           _currentGameSpeed = 1f; // 게임 속도
    private float           _maxGameSpeed = 1.5f;   // 게임 최고 속도

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _game = Managers.Game;

        BindObject(typeof(GameObjects));
        BindSlider(typeof(Sliders));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        _mercenaryTabScroll = GetObject((int)GameObjects.MercenaryTab).GetComponent<ScrollRect>();

        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(OnClickPauseButton);
        GetButton((int)Buttons.GameSpeedButton).gameObject.BindEvent(OnClickGameSpeedButton);
        GetButton((int)Buttons.AbilityListButton).gameObject.BindEvent(OnClickAbilityListButton);
        GetButton((int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);

        GetButton((int)Buttons.MercenaryButton).gameObject.BindEvent((PointerEventData eventData)=>{ ShowTab(PlayTab.Mercenary); });
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent((PointerEventData eventData)=>{ ShowTab(PlayTab.Upgrade); });

        // Test 버튼
        GetButton((int)Buttons.TestRegistarButton).onClick.AddListener(()=>{
            MercenaryRegister(Managers.Data.Mercenarys[UnityEngine.Random.Range(21, 30)].MercenaryClone<MercenaryStat>(), 10);
        });

        Managers.Game.GameScene.ActiveStartButton(false);

        _game.OnEnemySpawnEvent -= RefreshEnemyBar;
        _game.OnEnemySpawnEvent += RefreshEnemyBar;

        // 용병 슬롯 초기화
        foreach(Transform child in GetObject((int)GameObjects.MercenaryContent).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 강화 버튼 채우기
        PopulateUpgradeButton();

		Managers.Resource.Instantiate("UI/SubItem/UI_DragSlot", transform);

        SetEventHandler();

        ShowTab(PlayTab.Mercenary);

        OnRPSPopup();

        return true;
    }

    public  void OnRPSPopup()       { Invoke("OnDelayRPSPopup", 1f); }
    private void OnDelayRPSPopup()  { Managers.UI.ShowPopupUI<UI_RPSPopup>().RefreshUI(); }

    public void SetNextWave(WaveData waveData)
    {
        _wave = waveData;

        GetSlider((int)Sliders.EnemySlider).minValue = 0;
        GetSlider((int)Sliders.EnemySlider).maxValue = _wave.maxEnemyCount;
        GetSlider((int)Sliders.EnemySlider).value = _wave.maxEnemyCount;

        GetText((int)Texts.EnemyCountText).text = $"{_wave.maxEnemyCount} / {_wave.maxEnemyCount}";
        GetText((int)Texts.WaveTimeText).text = string.Format("{0:N2}", 20f);

        RefreshWaveInfo();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;
    }

    // 웨이브 시간
    private float noTime = 10f;
    public void RefreshWaveTime(bool isFormat, float time)
    {
        // 색 설정 (noTime 부터 빨간색)
        GetText((int)Texts.WaveTimeText).color = time <= noTime && isFormat ? Color.red : Color.white;

        // 시간 설정
        GetText((int)Texts.WaveTimeText).text = isFormat ? string.Format("{0:N2}", time) : (Mathf.CeilToInt(time)).ToString();
    }

    // 남은 몬스터
    public void RefreshEnemyBar(int count)
    {
        // 음수라면
        if (count > 0)
            return;

        _game.RemainEnemyCount += count;

        if (float.IsNaN(_game.RemainEnemyCount) == true)
            GetSlider((int)Sliders.EnemySlider).value = 0;
        else
            GetSlider((int)Sliders.EnemySlider).value = _game.RemainEnemyCount;

        GetText((int)Texts.EnemyCountText).text = $"{_game.RemainEnemyCount} / {_wave.maxEnemyCount}";
    }

    // 웨이브 정보
    public void RefreshWaveInfo()
    {
        GetText((int)Texts.EnemyHpText).text = _wave.hp.ToString();
        GetText((int)Texts.EnemyShieldText).text = _wave.shield.ToString();
        GetText((int)Texts.EnemyDefenceText).text = _wave.defence.ToString();
        GetText((int)Texts.WaveLevelText).text = $"{_wave.waveLevel} / 100";
    }

    // 골드
    public Action _onRefreshGoldAction;
    public void RefreshGold(int goldCount = 0)
    {
        if (_init == false)
            return;

        GetText((int)Texts.GoldText).text = _game.GameGold > 0 ? Utils.GetCommaText(_game.GameGold) : "0";

        // 골드를 획득하면 Effect Text 생성
        if (goldCount > 0)
        {
            DamageNumber goldText = Managers.Resource.Load<DamageNumber>("Prefabs/Text/Gold").Spawn(Vector3.zero, goldCount);
            goldText.SetAnchoredPosition(GetObject((int)GameObjects.StatusGold).transform, new Vector2(0, 0));
        }

        _onRefreshGoldAction?.Invoke();
    }

    public void ShowTab(PlayTab tab)
    {
        if (_tab == tab)
            return;

        _tab = tab;

        Color darkGrayColor = new Color(121f/255f, 110f/255f, 111f/255f);   // 어두운 회색
        Color yellowColor = new Color(246f/255f, 225f/255f, 156f/255f);     // 밝은 노랑색

        GetObject((int)GameObjects.MercenaryTab).SetActive(false);
        GetObject((int)GameObjects.UpgradeTab).SetActive(false);
        GetObject((int)GameObjects.MercenaryFocus).SetActive(false);
        GetObject((int)GameObjects.UpgradeFocus).SetActive(false);
        GetText((int)Texts.MercenaryText).color = darkGrayColor;
        GetText((int)Texts.UpgradeText).color = darkGrayColor;

        switch(_tab)
        {
            case PlayTab.Mercenary:
                GetObject((int)GameObjects.MercenaryTab).SetActive(true);
                GetObject((int)GameObjects.MercenaryTab).GetComponent<ScrollRect>().ResetHorizontal(0);
                GetObject((int)GameObjects.MercenaryFocus).SetActive(true);
                GetText((int)Texts.MercenaryText).color = yellowColor;
                break;
            case PlayTab.Upgrade:
                GetObject((int)GameObjects.UpgradeTab).SetActive(true);
                GetObject((int)GameObjects.UpgradeFocus).SetActive(true);
                GetText((int)Texts.UpgradeText).color = yellowColor;
                break;
        }
    }

    // 용병 슬롯 등록
    public void MercenaryRegister(MercenaryStat mercenaryStat, int count = 1)
    {
        // 용병 정보에 맞는 슬롯 탐지
        UI_MercenarySlot slot = GetMercenarySlot(mercenaryStat, true);
        if (slot.IsFakeNull() == false)
        {
            slot.SetCount(count);
            return;
        }

        // 중복된 용병 슬롯이 없으면 생성하여 저장
        UI_MercenarySlot item = Managers.UI.MakeSubItem<UI_MercenarySlot>(GetObject((int)GameObjects.MercenaryContent).transform);
        item.SetInfo(mercenaryStat, count);

        _mercenarySlots.Add(item);

        SortMercenarySlot();
    }

    // 용병 슬롯 삭제
    public void RemoveMercenarySlot(UI_MercenarySlot slot) { _mercenarySlots.Remove(slot); }

    // 용병 슬롯 정렬
    public void SortMercenarySlot()
    {
        if (_mercenarySlots.Count < 2)
            return;

        // 내림차순 정렬
        _mercenarySlots.Sort((slot1, slot2) =>
        {
            // 1. 등급으로 정렬
            if (slot1._mercenary.Grade != slot2._mercenary.Grade)
            {
                return slot2._mercenary.Grade.CompareTo(slot1._mercenary.Grade);
            }
            
            // 2. 진화 수준으로 정렬
            if (slot1._mercenary.CurrentEvolution != slot2._mercenary.CurrentEvolution)
            {
                return slot2._mercenary.CurrentEvolution.CompareTo(slot1._mercenary.CurrentEvolution);
            }
            
            // 3. 개수로 정렬
            return slot2._itemCount.CompareTo(slot1._itemCount);
        });

        for(int i=0; i<_mercenarySlots.Count; i++)
            _mercenarySlots[i].transform.SetSiblingIndex(i);
    }

    private void PopulateUpgradeButton()
    {
        Transform grid = GetObject((int)GameObjects.UpgradeGrid).transform;

        foreach(Transform child in grid)
            Managers.Resource.Destroy(child.gameObject);
        
        for(int i=1; i<(int)Define.RaceType.MaxMercenary; i++)
        {
            UI_UpgradeButton upgradeButton = Managers.UI.MakeSubItem<UI_UpgradeButton>(grid);
            upgradeButton.SetInfo((Define.RaceType)i);
        }
    }

    private void OnClickPauseButton(PointerEventData eventData)
    {
        Debug.Log("OnClickPauseButton");
        
        Time.timeScale = 0f;
        Managers.UI.ShowPopupUI<UI_PausePopup>().SetInfo(_currentGameSpeed);
    }

    private void OnClickGameSpeedButton(PointerEventData eventData)
    {
        Debug.Log("OnClickGameSpeedButton");

        _currentGameSpeed += 0.5f;

        if (_currentGameSpeed > _maxGameSpeed)
            _currentGameSpeed = 1f;

        GetText((int)Texts.GameSpeedButtonText).text = $"{_currentGameSpeed}X";
        Time.timeScale = _currentGameSpeed;
    }

    private void OnClickAbilityListButton(PointerEventData eventData)
    {
        Debug.Log("OnClickAbilityListButton");

        
    }

    private void OnClickStartButton(PointerEventData eventData)
    {
        Debug.Log("OnClickStartButton");

        // 웨이브 시작
        Managers.Game.WaveSystem.WaveStart();

        ActiveStartButton(false);
    }

    private void SetEventHandler()
    {
        // 용병 슬롯 탭 Drop 설정
        GetObject((int)GameObjects.MercenaryTab).BindEvent((PointerEventData eventData)=>
        {
            UI_DragSlot dragSlot = UI_DragSlot.instance;

            // 드래그 정보가 존재하는가?
            if (dragSlot.GetMercenary().IsNull() == true)
                return;

            // 이미 등록된 슬롯인가?
            if (IsSlotCheck(dragSlot.itemSlot as UI_MercenarySlot) == true)
                return;

            // 용병 슬롯 등록
            MercenaryRegister(dragSlot.GetMercenary());

            // 타일에서 왔으면 타일 초기화
            if (dragSlot.mercenaryTile.IsFakeNull() == false)
            {
                Managers.Game.Despawn(dragSlot.mercenaryTile._mercenary);
                dragSlot.mercenaryTile.Clear();
            }

        }, Define.UIEvent.Drop);
    }

    // Start Button 활성화 여부
    public void ActiveStartButton(bool isActive) { GetButton((int)Buttons.StartButton).gameObject.SetActive(isActive); }

    // 용병으로 슬롯 찾기
    public UI_MercenarySlot GetMercenarySlot(MercenaryStat mercenary, bool isEvolution = true)
    {
        // 용병의 정보를 토대로 슬롯 찾기
        foreach(UI_MercenarySlot slot in _mercenarySlots)
        {
            if (slot._mercenary.IsSameMercenary(mercenary, isEvolution) == true)
                return slot;
        }

        return null;
    }

    // 슬롯이 존재하는지 확인
    public bool IsSlotCheck(UI_MercenarySlot mercenarySlot)
    {
        if (mercenarySlot.IsFakeNull() == true)
            return false;

        foreach(UI_MercenarySlot slot in _mercenarySlots)
        {
            if (slot == mercenarySlot)
                return true;
        }

        return false;
    }
}
