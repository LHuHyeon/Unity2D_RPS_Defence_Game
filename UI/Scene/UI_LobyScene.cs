using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LobyScene : UI_Scene
{
    enum GameObjects
    {
        HomeBar,
        MercenaryBar,
        ShopBar,
    }

    enum Buttons
    {
        HomeButton,
        MercenaryButton,
        ShopButton,
        RankButton,
    }

    enum Texts
    {
        NameText,
        GoldText,
        GemText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
