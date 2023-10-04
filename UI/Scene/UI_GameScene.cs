using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        MercenaryTab,
        MercenaryContent,
        UIDetector,
        StatusGold,
    }

    enum Sliders
    {
        EnemySlider,
    }

    enum Buttons
    {
        PauseButton,
        GameSpeedButton,
    }
    
    enum Texts
    {
        WaveTimeText,
        EnemyHpText,
        EnemyDefenceText,
        WaveLevelText,
        GoldText,
        EnemyCountText,
        GameSpeedButtonText,
    }

    enum PlayTab
	{
		None,
		Mercenary,
		Upgrade,
		Mix,
	}

    private PlayTab         _tab = PlayTab.None;
    
    private GameManagerEx   _game;
    private WaveData        _wave;

    private List<UI_MercenaryItem> _MercenaryItems = new List<UI_MercenaryItem>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _game = Managers.Game;

        BindObject(typeof(GameObjects));
        BindSlider(typeof(Sliders));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.PauseButton).onClick.AddListener(OnClickPauseButton);
        GetButton((int)Buttons.GameSpeedButton).onClick.AddListener(OnClickGameSpeedButton);

        _game.OnEnemySpawnEvent -= RefreshEnemyBar;
        _game.OnEnemySpawnEvent += RefreshEnemyBar;

        PopulateMercenary();

		Managers.Resource.Instantiate("UI/SubItem/UI_DragSlot", transform);

        SetEventHandler();

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

    }

    // 웨이브 시간
    public void RefreshWaveTime(bool isFormat, float time)
    {
        if (isFormat == true)
            GetText((int)Texts.WaveTimeText).text = string.Format("{0:N2}", time);
        else
            GetText((int)Texts.WaveTimeText).text = (Mathf.CeilToInt(time)).ToString();
    }

    // 남은 몬스터
    public void RefreshEnemyBar(int count)
    {
        // 음수라면
        if (count > 0)
            return;

        _game.remainEnemys += count;

        if (float.IsNaN(_game.remainEnemys) == true)
            GetSlider((int)Sliders.EnemySlider).value = 0;
        else
            GetSlider((int)Sliders.EnemySlider).value = _game.remainEnemys;

        GetText((int)Texts.EnemyCountText).text = $"{_game.remainEnemys} / {_wave.maxEnemyCount}";
    }

    // 웨이브 정보
    public void RefreshWaveInfo()
    {
        GetText((int)Texts.EnemyHpText).text = _wave.hp.ToString();
        GetText((int)Texts.EnemyDefenceText).text = _wave.defence.ToString();
        GetText((int)Texts.WaveLevelText).text = $"{_wave.waveLevel} / 100";
    }

    // 골드
    public void RefreshGold(int goldCount = 0)
    {
        if (_init == false)
            return;

        GetText((int)Texts.GoldText).text = Utils.GetCommaText(_game.GameGold);

        // 골드를 획득하면 Effect Text 생성
        if (goldCount > 0)
        {
            DamageNumber goldText = Managers.Resource.Load<DamageNumber>("Prefabs/Text/Gold").Spawn(Vector3.zero, goldCount);
            goldText.SetAnchoredPosition(GetObject((int)GameObjects.StatusGold).transform, new Vector2(0, 0));
        }
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

    private void OnClickPauseButton()
    {
        Debug.Log("OnClickPauseButton");

        // TODO : 게임 일시 정지
    }

    private void OnClickGameSpeedButton()
    {
        Debug.Log("OnClickGameSpeedButton");

        // TODO : 게임 속도 상승 or 다운
    }

    private void SetEventHandler()
    {
        // 2D Object를 위한 UI 탐지 Event 설정
        GameObject go = GetObject((int)GameObjects.UIDetector);

        go.BindEvent((PointerEventData eventData)=>
        {
            if (UI_DragSlot.instance.GetMercenary().IsNull() == true)
                return;

            UI_DragSlot.instance.SetColor(1);
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
            UI_DragSlot.instance.ClearSlot();
            Managers.Game.isDrag = false;

        }, Define.UIEvent.EndDrag);

        // 용병 슬롯 탭 Drop 설정
        GetObject((int)GameObjects.MercenaryTab).BindEvent((PointerEventData eventData)=>
        {
            UI_DragSlot dragSlot = UI_DragSlot.instance;

            // 드래그 정보가 존재하는가?
            if (dragSlot.GetMercenary().IsNull() == true)
                return;

            // 이미 등록된 슬롯인가?
            if (IsSlotCheck(dragSlot.itemSlot as UI_MercenaryItem) == true)
                return;

            // 용병 슬롯 등록
            MercenaryRegister(dragSlot.GetMercenary());

            // 타일에서 왔으면 타일 초기화
            if (dragSlot.tile.IsFakeNull() == false)
            {
                Managers.Resource.Destroy(dragSlot.tile.mercenaryObj);
                dragSlot.tile.Clear();
            }

            dragSlot.DragInfoClear();

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
