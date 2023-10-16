using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MercenaryInfoPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
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

    public MercenaryStat _mercenary;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

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

    }

    public void Clear()
    {
        _mercenary = null;
    }

    private void OnClickExitButton()
    {
        Debug.Log("OnClickExitButton");
        
        Managers.UI.ClosePopupUI(this);
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
