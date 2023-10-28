using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_MercenaryInfoPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        ExitBackground,
        StarGrid,
        EvolutionTextGrid,
        EvolutionGauge,
    }

    enum Buttons
    {
        SaleButton,
        EvolutionButton,
    }

    enum Images
    {
        Icon,
        IconBackground,
    }

    enum Texts
    {
        RaceText,
        JobText,
        InfoText,
        SaleGoldText,
        EvolutionGaugeText,
        EvolutionButtonText,
    }

    public MercenaryTile    _tile;
    public UI_MercenarySlot _slot;

    public MercenaryStat    _mercenary;

    private int             _evolutionPlanCount = 0;    // 진화 목표수
    private int             _mercenarySalePrice = 0;    // 용병 판매 금액

    private bool            _isActive       = false;    // 팝업 활성화 여부
    private bool            _isEvolution    = false;    // 진화 가능 여부
    
    private List<Image>             _starIcons = new List<Image>();
    private List<UI_EvolutionText>  _evolutionTexts = new List<UI_EvolutionText>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.ExitBackground).BindEvent(OnClickExitButton);
        GetButton((int)Buttons.SaleButton).gameObject.BindEvent(OnClickSaleButton);
        GetButton((int)Buttons.EvolutionButton).gameObject.BindEvent(OnClickEvolutionButton);
        
        // 진화 등급(별) Icon 가져오기
        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
            _starIcons.Add(child.GetComponent<Image>());

        PopulateEvolutionText();    // 진화 능력 Text 채우기

        RefreshUI();

        return true;
    }

    // 타일 정보를 받아올 때
    public void SetInfoTile(MercenaryTile tile)
    {
        _tile = tile;
        _mercenary = _tile.GetMercenary().GetStat();

        _slot = null;
        RefreshUI();
    }

    // 슬롯 정보를 받아올 때
    public void SetInfoSlot(UI_MercenarySlot slot)
    {
        _slot = slot;
        _mercenary = _slot._mercenary;

        _tile = null;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (_mercenary.IsNull() == true)
            return;

        _mercenary.RefreshAddData();

        RefreshInfo();
        RefreshEvolution();

        // 팝업이 비활성화일 때 호출이면 코루틴 작동
        if (_isActive == false)
            StartCoroutine(CallPopup());
    }

    public void RefreshInfo()
    {
        _mercenarySalePrice = _mercenary.SalePrice * ((int)_mercenary.CurrentEvolution+1);

        GetImage((int)Images.IconBackground).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_Grade_"+_mercenary.Grade.ToString());
        GetImage((int)Images.Icon).sprite = _mercenary.Icon;

        GetText((int)Texts.RaceText).text       = $@"종족 <color={GetRaceColor()}>{_mercenary.Race.ToString()}</color>";
        GetText((int)Texts.JobText).text        = $@"직업 <color={GetJobColor()}>{_mercenary.Job.ToString()}</color>";
        GetText((int)Texts.SaleGoldText).text   = _mercenarySalePrice.ToString();

        string addDamageText        = _mercenary.AddDamage > 0 ? $@"<color=green>[+{_mercenary.AddDamage}]</color>" : "";
        string addAttackRateText    = _mercenary.AddAttackRate > 0 ? $@"<color=green>[+{_mercenary.AddAttackRate}]</color>" : "";
        string addAttackRangeText   = _mercenary.AddAttackRange > 0 ? $@"<color=green>[+{_mercenary.AddAttackRange}]</color>" : "";

        GetText((int)Texts.InfoText).text = 
$@"등급 <color={GetGradeColor()}>{_mercenary.Grade.ToString()}</color>
공격력 {_mercenary.Damage}{addDamageText}
공격속도 {_mercenary.AttackSpeed}{addAttackRateText}
사거리 {_mercenary.AttackRange}{addAttackRangeText}";
        
        // 진화 능력 Text 적용
        for(int i=0; i<_evolutionTexts.Count; i++)
            _evolutionTexts[i].SetInfo(_mercenary.Buffs[i], _mercenary.CurrentEvolution);
    }

    public void RefreshEvolution()
    {
        Slider  evolutionSlider = GetObject((int)GameObjects.EvolutionGauge).GetComponent<Slider>();
        int     mercenaryCount  = Managers.Game.GameScene.GetMercenarySlot(_mercenary, false)?._itemCount ?? 0;
        
        _evolutionPlanCount     = ((int)_mercenary.CurrentEvolution + 1);

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = _evolutionPlanCount;

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

        // 용병 진화 수 만큼 별 활성화
        for(int i=0; i<((int)_mercenary.CurrentEvolution); i++)
            _starIcons[i].sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_Star");

        // 나머지 별 비활성화
        for(int i=((int)_mercenary.CurrentEvolution); i<_starIcons.Count; i++)
            _starIcons[i].sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_DeStar");

        // 타일에서의 용병은 진화 불가
        GetButton((int)Buttons.EvolutionButton).gameObject.SetActive(_tile.IsNull());
    }

    private int _maxStarCount = 3;  // 진화 별 최대 개수
    private void PopulateEvolutionText()
    {
        Transform parent = GetObject((int)GameObjects.EvolutionTextGrid).transform;

        foreach(Transform child in parent)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=1; i<=_maxStarCount; i++)
        {
            UI_EvolutionText evolution = Managers.UI.MakeSubItem<UI_EvolutionText>(parent);
            evolution._evolutionType = (Define.EvolutionType)i;

            _evolutionTexts.Add(evolution);
        }
    }

    private void OnClickEvolutionButton(PointerEventData eventData)
    {
        Debug.Log("OnClickEvolutionButton");

        if (_isEvolution == false)
            return;

        // 진화 재료로 사용될 slot 가져오기
        UI_MercenarySlot slot = Managers.Game.GameScene.GetMercenarySlot(_mercenary, false);
        if (slot.IsNull() == true)
            return;

        // 진화 목표수 만큼 차감
        slot.SetCount(-_evolutionPlanCount);

        // 첫 진화된 용병은 다른 슬롯에 등록
        if (_mercenary.CurrentEvolution == Define.EvolutionType.Unknown)
        {
            _mercenary = Managers.Data.Mercenarys[_mercenary.Id].MercenaryClone();
            _mercenary.CurrentEvolution++;

            Managers.Game.GameScene.MercenaryRegister(_mercenary, 1);
        }
        else
            _mercenary.CurrentEvolution++;

        Managers.Game.GameScene.GetMercenarySlot(_mercenary, true)?.RefreshUI();

        RefreshUI();
    }

    private void OnClickSaleButton(PointerEventData eventData)
    {
        Debug.Log("OnClickSaleButton");

        string saleText = Define.SaleConfirmText + "\n" + $@"<color=yellow>Gold {_mercenarySalePrice}</color>";

        Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
        {
            Managers.Game.GameGold += _mercenarySalePrice;

            if (_tile.IsFakeNull() == false)
            {
                Managers.Game.Despawn(_tile._mercenary);
                _tile.Clear();
            }
            else if (_slot.IsFakeNull() == false)
                _slot.SetCount(-1);

            Clear();
        }, saleText);
    }

    private void OnClickExitButton(PointerEventData eventData)
    {
        Debug.Log("OnClickExitButton");
        
        if (_isActive == true)
            StartCoroutine(ExitPopup());
    }

    private float lerpTime      = 0.5f;  // Lerp 시간
    private float maxAlpha      = 0.7f;  // 투명도 최대치
    private Vector3 startPos    = Vector3.up * 400f;
    private Vector3 endPos      = Vector3.up * 300f;
    private IEnumerator CallPopup()
    {
        _isActive = true;

        Image icon = GetObject((int)GameObjects.Background).GetComponent<Image>();

        SetColor(icon, 0);

        // 내려오며 팝업 등장
        float currentTime = 0;
        float currentAlpha = 0f;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;

            float t = currentTime / lerpTime;

            t = Mathf.Sin(t * Mathf.PI * 0.5f);     // SmoothStep 참고 

            GetObject((int)GameObjects.Background).transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            // 점점 밝아지게
            if (currentAlpha < maxAlpha)
            {
                currentAlpha += 0.05f;
                SetColor(icon, currentAlpha);
            }

            yield return null;
        }

        SetColor(icon, maxAlpha);
    }

    private IEnumerator ExitPopup()
    {
        _isActive = false;

        Image icon = GetObject((int)GameObjects.Background).GetComponent<Image>();

        // 올라가며 팝업 없애기
        float currentTime = 0;
        float currentAlpha = maxAlpha;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            
            t = Mathf.Sin(t * Mathf.PI * 0.5f);     // SmoothStep 참고 

            GetObject((int)GameObjects.Background).transform.localPosition = Vector3.Lerp(endPos, startPos, t);

            // 점점 밝아지게
            currentAlpha -= 0.05f;
            SetColor(icon, currentAlpha);

            yield return null;
        }

        SetColor(icon, 0);

        Clear();
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

    // 종족 컬러
    private string GetRaceColor()
    {
        string colorName;

        switch (_mercenary.Race)
        {
            case Define.RaceType.Human:
                colorName = "#AED6F1";      // Light Blue
                break;
            case Define.RaceType.Elf:
                colorName = "#00FF00";      // Lime
                break;
            case Define.RaceType.WereWolf:
                colorName = "#9999CC";      // 구름색?
                break;
            default:
                colorName = "white";
                break;
        }

        return colorName;
    }

    // 직업 컬러
    private string GetJobColor()
    {
        string colorName;

        switch (_mercenary.Job)
        {
            case Define.JobType.Warrior:
                colorName = "#CCCC33";      // 짖은 노랑
                break;
            case Define.JobType.Archer:
                colorName = "#FF6666";      // 연한 빨강?
                break;
            case Define.JobType.Wizard:
                colorName = "#FF00FF";      // Fuchsia
                break;
            default:
                colorName = "white";
                break;
        }

        return colorName;
    }

    // 등급 컬러
    private string GetGradeColor()
    {
        string colorName;

        switch (_mercenary.Grade)
        {
            case Define.GradeType.Basic:
                colorName = "grey";
                break;
            case Define.GradeType.Common:
                colorName = "white";
                break;
            case Define.GradeType.UnCommon:
                colorName = "green";
                break;
            case Define.GradeType.Rare:
                colorName = "#0066FF";      // 밝은 파랑
                break;
            case Define.GradeType.Epic:
                colorName = "#CC33FF";      // 보라색
                break;
            case Define.GradeType.Legendary:
                colorName = "orange";
                break;
            default:
                colorName = "black";
                break;
        }

        return colorName;
    }

    public void Clear()
    {
        _isActive = false;
        _tile = null;
        _slot = null;
        _mercenary = null;
        Managers.UI.ClosePopupUI(this);
    }
}
