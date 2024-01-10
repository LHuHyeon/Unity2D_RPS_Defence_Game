using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_MercenaryViewSlot.cs
 * Desc :   "UI_RPSPopup"의 하위 항목으로 사용
 *          가위바위보 카드를 뽑은 후 어떤 용병을 획득하는지 미리보기 기능을 수행한다.
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : SetInfo()     - 정보 설정
 &  : RefreshUI()   - UI 새로고침
 &  : Clear()       - 초기화
 *
 */
 
public class UI_MercenaryViewSlot : UI_Base
{
    enum Images
    {
        Background,
        ViewIcon,
        JobLabel,
        JobLabelIcon,
    }

    public MercenaryStat    mercenaryStat;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));

        RefreshUI();

        return true;
    }

    // 보상이 확정일 때
    public void SetInfo(MercenaryStat stat)
    {
        mercenaryStat = stat;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        if (mercenaryStat.IsNull() == true)
            return;

        GetImage((int)Images.ViewIcon).sprite = mercenaryStat.Icon;
        GetImage((int)Images.Background).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_Grade_"+mercenaryStat.Grade.ToString());
        GetImage((int)Images.JobLabel).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Bg_JobIcon_"+mercenaryStat.Job.ToString());
        GetImage((int)Images.JobLabelIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Job_"+mercenaryStat.Job.ToString());
    }

    public void Clear()
    {
        mercenaryStat = null;
    }
}
