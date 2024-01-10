using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_Popup.cs
 * Desc :   모든 Popup의 부모
 */
 
public class UI_Popup : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
