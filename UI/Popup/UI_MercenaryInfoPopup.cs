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
        InformationBar,
        UI_Evolution,
    }

    enum Buttons
    {
        SaleButton,
        FoldButton,
    }

    enum Images
    {
        Icon,
        IconBackground,
        FoldIcon,
    }

    enum Texts
    {
        RaceText,
        JobText,
        InfoText,
        SaleGoldText,
    }

    public MercenaryTile    _tile;
    public UI_MercenarySlot _slot;

    public MercenaryStat    _mercenary;

    private int             _mercenarySalePrice = 0;    // 용병 판매 금액

    [SerializeField]
    private bool            _isActive = false;          // 팝업 활성화 여부
    [SerializeField]
    private bool            _isFold = false;            // 정보를 접은 상태

    private UI_Evolution    _evolution;
    
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
        GetButton((int)Buttons.FoldButton).gameObject.BindEvent(OnClickFoldButton);
        
        // 진화 등급(별) Icon 가져오기
        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
            _starIcons.Add(child.GetComponent<Image>());

        PopulateEvolution();    // 진화 정보 채우기

        RefreshUI();

        return true;
    }

    // 타일 정보를 받아올 때
    public void SetInfoTile(MercenaryTile tile)
    {
        _tile   = tile;
        _slot   = null;

        _mercenary = _tile.GetMercenary().GetStat();

        RefreshUI();
    }

    // 슬롯 정보를 받아올 때
    public void SetInfoSlot(UI_MercenarySlot slot)
    {
        _slot   = slot;
        _tile   = null;

        _mercenary  = _slot._mercenary;

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

        _evolution.SetInfo();

        // 팝업이 비활성화이고, 정보가 접힌 상태가 아니라면
        if (_isActive == false)
        {
            _isActive = true;

            if (_isFold == false)
                StartCoroutine(CallPopup());
        }
    }

    public void RefreshInfo()
    {
        _mercenarySalePrice = _mercenary.SalePrice * ((int)_mercenary.CurrentEvolution+1);

        GetImage((int)Images.IconBackground).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_Grade_"+_mercenary.Grade.ToString());
        GetImage((int)Images.Icon).sprite = _mercenary.Icon;

        GetText((int)Texts.RaceText).text       = $@"종족 <color={GetRaceColor()}>{_mercenary.Race.ToString()}</color>";
        GetText((int)Texts.JobText).text        = $@"직업 <color={GetJobColor()}>{_mercenary.Job.ToString()}</color>";
        GetText((int)Texts.SaleGoldText).text   = _mercenarySalePrice.ToString();

        // 추가 능력치 확인
        string addDamageText        = _mercenary.AddDamage > 0 ? $@"<color=green>[+{_mercenary.AddDamage}]</color>" : "";
        string addAttackRateText    = _mercenary.AddAttackRate > 0 ? $@"<color=green>[+{_mercenary.AddAttackRate}]</color>" : "";
        string addAttackRangeText   = _mercenary.AddAttackRange > 0 ? $@"<color=green>[+{_mercenary.AddAttackRange}]</color>" : "";

        // 능력치 정보 문자열
        GetText((int)Texts.InfoText).text = 
$@"등급 <color={GetGradeColor()}>{_mercenary.Grade.ToString()}</color>
공격력 {_mercenary.Damage}{addDamageText}
공격속도 {_mercenary.AttackSpeed}{addAttackRateText}
사거리 {_mercenary.AttackRange}{addAttackRangeText}";

        // 타일에서 정보가 왔으면 True
        _isFold = _tile.IsNull() == false ? true : false;
        OnFold();
        
        // 진화 능력 Text 적용
        for(int i=0; i<_evolutionTexts.Count; i++)
            _evolutionTexts[i].SetInfo(_mercenary.Buffs[i], _mercenary.CurrentEvolution);

        // 용병 진화 수 만큼 별 활성화
        for(int i=0; i<((int)_mercenary.CurrentEvolution); i++)
            _starIcons[i].sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_Star");

        // 나머지 별 비활성화
        for(int i=((int)_mercenary.CurrentEvolution); i<_starIcons.Count; i++)
            _starIcons[i].sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_DeStar");
    }

    private int _maxStarCount = 3;  // 진화 별 최대 개수
    private void PopulateEvolution()
    {
        // 진화 정보 Text 객체 가져오기
        Transform parent = GetObject((int)GameObjects.EvolutionTextGrid).transform;

        foreach(Transform child in parent)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=1; i<=_maxStarCount; i++)
        {
            UI_EvolutionText evolution = Managers.UI.MakeSubItem<UI_EvolutionText>(parent);
            evolution._evolutionType = (Define.EvolutionType)i;

            _evolutionTexts.Add(evolution);
        }

        // 진화 진행 객체 가져오기
        _evolution = GetObject((int)GameObjects.UI_Evolution).GetComponent<UI_Evolution>();
        _evolution._infoPopup = this;
    }

    private void OnClickSaleButton(PointerEventData eventData)
    {
        Debug.Log("OnClickSaleButton");

        string saleText = Define.SaleConfirmText + "\n" + $@"<color=yellow>Gold {_mercenarySalePrice}</color>";

        // 확인 Popup 생성
        Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
        {
            //* <-- 확인을 눌렀을 때 실행되는 기능 -->

            // 판매 금액 받기
            Managers.Game.GameGold += _mercenarySalePrice;

            if (_tile.IsFakeNull() == false)
            {
                // 타일의 용병 정보라면 삭제
                Managers.Game.Despawn(_tile._mercenary);
                _tile.Clear();
                Clear();
            }
            else if (_slot.IsFakeNull() == false)
            {
                // 슬롯의 용병 정보라면 -1 차감
                _slot.SetCount(-1);

                if (_slot.IsFakeNull() == true)
                    Clear();
            }

        }, saleText);
    }

    private void OnClickFoldButton(PointerEventData eventData)
    {
        Debug.Log("OnClickFoldButton");

        _isFold = !_isFold;
        OnFold();
    }

    private void OnFold()
    {
        int     posY            = _isFold == true ? 800 : 300;          // Background Pos Y
        int     bgHeight        = _isFold == true ? 415 : 950;          // Background Height
        int     bgExitHeight    = _isFold == true ? 430 : 1380;         // Exit Background Height
        string  iconPathName    = _isFold == true ? "Down" : "Up";      // Icon Sprite Path Name

        // Background 크기, 높이 설정
        RectTransform rectBg = GetObject((int)GameObjects.Background).GetComponent<RectTransform>();
        rectBg.anchoredPosition = new Vector2(0, posY);
        rectBg.sizeDelta        = new Vector2(rectBg.sizeDelta.x, bgHeight);

        // Exit Background 크기 설정
        rectBg = GetObject((int)GameObjects.ExitBackground).GetComponent<RectTransform>();
        rectBg.sizeDelta = new Vector2(rectBg.sizeDelta.x, bgExitHeight);

        // Background의 Mask 비활성화
        GetObject((int)GameObjects.Background).GetComponent<Mask>().enabled = false;

        // Fold Icon Sprite 설정
        GetImage((int)Images.FoldIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Fold_" + iconPathName);

        // 정보Bar 활성화/비활성화 설정
        GetObject((int)GameObjects.InformationBar).SetActive(!_isFold);
    }

    private void OnClickExitButton(PointerEventData eventData)
    {
        Debug.Log("OnClickExitButton");
        
        if (_isActive == true)
        {
            if (_isFold == true)
                Clear();
            else
                StartCoroutine(ExitPopup());
        }
    }

    private float lerpTime      = 0.5f;  // Lerp 시간
    private float maxAlpha      = 0.7f;  // 투명도 최대치
    private Vector3 startPos    = Vector3.up * 400f;
    private Vector3 endPos      = Vector3.up * 300f;
    private IEnumerator CallPopup()
    {
        // Background의 Mask 활성화
        GetObject((int)GameObjects.Background).GetComponent<Mask>().enabled = true;

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
        
        // Background의 Mask 활성화
        GetObject((int)GameObjects.Background).GetComponent<Mask>().enabled = true;

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
                colorName = "#CCCC33";      // 짙은 노랑
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
