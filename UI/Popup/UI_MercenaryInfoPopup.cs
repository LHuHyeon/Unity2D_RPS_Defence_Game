using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MercenaryInfoPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        ExitBackground,
    }

    enum Buttons
    {
        ExitButton,
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
    }

    public MercenaryStat    _mercenary;

    private bool            _isActive = false;  // 팝업 활성화 여부

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.ExitBackground).BindEvent(OnClickExitButton);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);

        RefreshUI();

        return true;
    }

    public void SetInfo(MercenaryStat mercenary)
    {
        _mercenary = mercenary;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (_mercenary.IsNull() == true)
            return;

        GetImage((int)Images.IconBackground).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_Grade_"+_mercenary.Grade.ToString());
        GetImage((int)Images.Icon).sprite = _mercenary.Icon;

        GetText((int)Texts.RaceText).text = $@"종족 <color={GetRaceColor()}>{_mercenary.Race.ToString()}</color>";
        GetText((int)Texts.JobText).text = $@"직업 <color={GetJobColor()}>{_mercenary.Job.ToString()}</color>";
        GetText((int)Texts.InfoText).text = 
$@"등급 <color={GetGradeColor()}>{_mercenary.Grade.ToString()}</color>
공격력 {_mercenary.Damage.ToString()}
공격속도 {_mercenary.AttackRate.ToString()}
사거리 {_mercenary.AttackRange.ToString()}";

        if (_isActive == false)
            StartCoroutine(CallPopup());
    }

    public void Clear()
    {
        _isActive = false;
        _mercenary = null;
        Managers.UI.ClosePopupUI(this);
    }

    private void OnClickExitButton(PointerEventData eventData)
    {
        Debug.Log("OnClickExitButton");
        
        if (_isActive == true)
            StartCoroutine(ExitPopup());
    }

    private float lerpTime = 0.5f;  // Lerp 시간
    private Vector3 startPos = Vector3.up * 400f;
    private Vector3 endPos = Vector3.up * 300f;
    private IEnumerator CallPopup()
    {
        _isActive = true;

        // 내려오며 팝업 등장
        float currentTime = 0;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;

            float t = currentTime / lerpTime;

            t = Mathf.Sin(t * Mathf.PI * 0.5f);     // SmoothStep 참고 

            GetObject((int)GameObjects.Background).transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
    }

    private IEnumerator ExitPopup()
    {
        _isActive = false;

        // 올라가며 팝업 없애기
        float currentTime = 0;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            
            t = Mathf.Sin(t * Mathf.PI * 0.5f);     // SmoothStep 참고 

            GetObject((int)GameObjects.Background).transform.localPosition = Vector3.Lerp(endPos, startPos, t);

            yield return null;
        }

        Clear();
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
}
