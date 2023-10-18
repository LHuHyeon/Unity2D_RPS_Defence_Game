using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ConfirmPopup : UI_Popup
{
    enum GameObjects
    {
        ExitBackground,
        Background,
    }

    enum Buttons
    {
        ConfirmButton,
        CloseButton,
    }

    enum Texts
    {
        ConfirmText,
    }

    private string _descripition;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.ExitBackground).BindEvent(OnClickCloseButton);

        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        RefreshUI();

        return true;
    }

    private Action _onClickConfirmButton;
    public void SetInfo(Action onClickConfirmButton, string descripition)
    {
        _onClickConfirmButton = onClickConfirmButton;
        _descripition = descripition;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        GetText((int)Texts.ConfirmText).text = _descripition;
    }

    private void OnClickConfirmButton(PointerEventData eventData)
    {
        Debug.Log("OnClickConfirmButton");

        Clear();

        if (_onClickConfirmButton.IsNull() == false)
            _onClickConfirmButton.Invoke();
    }

    private void OnClickCloseButton(PointerEventData eventData)
    {
        Debug.Log("OnClickCloseButton");

        Clear();
    }

    public void Clear()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
