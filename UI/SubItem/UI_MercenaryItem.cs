using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MercenaryItem : UI_Base
{
    enum Images
    {
        Background,
        Icon,
    }

    enum Texts
    {
        ItemCount,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        return true;
    }

    // TODO : 용병 정보 받기
    public void SetInfo()
    {

    }

    public void RefreshUI()
    {
        if (_init == false)
            return;
    }
}
