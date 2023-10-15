using System.Collections;
using System.Collections.Generic;
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
        CompositionTab,
        MercenaryContent,
        StatusGold,
        MercenaryFocus,
        UpgradeFocus,
        CompositionFocus,
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
        TestRegistarButton,
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
        MercenaryText,
        UpgradeText,
        CompositionText,
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

    private PlayTab         _tab = PlayTab.None;

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

        GetButton((int)Buttons.MercenaryButton).gameObject.BindEvent((PointerEventData eventData)=>{ ShowTab(PlayTab.Mercenary); });
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent((PointerEventData eventData)=>{ ShowTab(PlayTab.Upgrade); });
        GetButton((int)Buttons.CompositionButton).gameObject.BindEvent((PointerEventData eventData)=>{ ShowTab(PlayTab.Composition); });

        GetButton((int)Buttons.TestRegistarButton).onClick.AddListener(()=>{
            MercenaryRegister(Managers.Data.Mercenarys[Random.Range(1, 40)]);
        });

        _game.OnEnemySpawnEvent -= RefreshEnemyBar;
        _game.OnEnemySpawnEvent += RefreshEnemyBar;

        // 용병 슬롯 초기화
        foreach(Transform child in GetObject((int)GameObjects.MercenaryContent).transform)
            Managers.Resource.Destroy(child.gameObject);

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

    }

    // 웨이브 시간
    private float noTime = 10f;
    public void RefreshWaveTime(bool isFormat, float time)
    {
        // 색 설정 (noTime(10f)부터 빨간색)
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

    public void ShowTab(PlayTab tab)
    {
        if (_tab == tab)
            return;

        _tab = tab;

        Color darkGrayColor = new Color(121f/255f, 110f/255f, 111f/255f);   // 어두운 회색
        Color yellowColor = new Color(246f/255f, 225f/255f, 156f/255f);     // 밝은 노랑색

        GetObject((int)GameObjects.MercenaryTab).SetActive(false);
        GetObject((int)GameObjects.UpgradeTab).SetActive(false);
        GetObject((int)GameObjects.CompositionTab).SetActive(false);
        GetObject((int)GameObjects.MercenaryFocus).SetActive(false);
        GetObject((int)GameObjects.UpgradeFocus).SetActive(false);
        GetObject((int)GameObjects.CompositionFocus).SetActive(false);
        GetText((int)Texts.MercenaryText).color = darkGrayColor;
        GetText((int)Texts.UpgradeText).color = darkGrayColor;
        GetText((int)Texts.CompositionText).color = darkGrayColor;

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
            case PlayTab.Composition:
                GetObject((int)GameObjects.CompositionTab).SetActive(true);
                GetObject((int)GameObjects.CompositionFocus).SetActive(true);
                GetText((int)Texts.CompositionText).color = yellowColor;
                break;
        }
    }

    // 용병 슬롯 등록
    public void MercenaryRegister(MercenaryStat mercenaryStat, int count = 1)
    {
        // 현재 존재하는 용병 슬롯 탐지
        foreach(UI_MercenarySlot slot in _mercenarySlots)
        {
            if (slot._mercenary == mercenaryStat)
            {
                slot.SetCount(count);
                return;
            }
        }

        // 중복된 용병 슬롯이 없으면 생성하여 저장
        UI_MercenarySlot item = Managers.UI.MakeSubItem<UI_MercenarySlot>(GetObject((int)GameObjects.MercenaryContent).transform);
        item.SetInfo(mercenaryStat);

        _mercenarySlots.Add(item);

        // TODO : 용병 정렬 구현 시 삭제
        GetObject((int)GameObjects.MercenaryTab).GetComponent<ScrollRect>().ResetHorizontal(1);
    }

    // 용병 슬롯 삭제
    public void RemoveMercenarySlot(UI_MercenarySlot slot) { _mercenarySlots.Remove(slot); }

    private void OnClickPauseButton(PointerEventData eventData)
    {
        Debug.Log("OnClickPauseButton");

        // TODO : 게임 일시 정지
    }

    private void OnClickGameSpeedButton(PointerEventData eventData)
    {
        Debug.Log("OnClickGameSpeedButton");

        // TODO : 게임 속도 상승 or 다운
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
                Managers.Resource.Destroy(dragSlot.mercenaryTile._mercenary);
                dragSlot.mercenaryTile.Clear();
            }

        }, Define.UIEvent.Drop);
    }

    // 슬롯이 존재하는지 확인
    public bool IsSlotCheck(UI_MercenarySlot mercenaryItem)
    {
        if (mercenaryItem.IsFakeNull() == true)
            return false;

        foreach(UI_MercenarySlot slot in _mercenarySlots)
        {
            if (slot == mercenaryItem)
                return true;
        }

        return false;
    }
}
