using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MercenaryViewSlot : UI_Base
{
    enum Images
    {
        Background,
        ViewIcon,
    }

    public MercenaryStat mercenaryStat;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));

        RefreshUI();

        return true;
    }

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
    }
}
