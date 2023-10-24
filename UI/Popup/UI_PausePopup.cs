using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PausePopup : UI_Popup
{
    enum GameObjects
    {
        ExitBackground,
    }

    enum Buttons
    {
        SettingButton,
        ContinueButton,
        GiveUpButton,
    }

    enum Texts
    {
        WaveText,
    }

    private float _currentGameSpeed;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetObject((int)GameObjects.ExitBackground).BindEvent((PointerEventData eventData) => { OnClickContinueButton(); });

        GetButton((int)Buttons.SettingButton).onClick.AddListener(OnClickSettingButton);
        GetButton((int)Buttons.ContinueButton).onClick.AddListener(OnClickContinueButton);
        GetButton((int)Buttons.GiveUpButton).onClick.AddListener(OnClickGiveUpButton);

        RefreshUI();

        return true;
    }

    public void SetInfo(float currentGameSpeed = 1f)
    {
        _currentGameSpeed = currentGameSpeed;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        GetText((int)Texts.WaveText).text = $"Wave {Managers.Game.CurrentWave.waveLevel}";
    }

    private void OnClickSettingButton()
    {
        Debug.Log("OnClickSettingButton");

        // TODO : 설정 구현
    }
    
    private void OnClickContinueButton()
    {
        Debug.Log("OnClickContinueButton");

        Managers.UI.ClosePopupUI(this);
        Time.timeScale = _currentGameSpeed;
    }

    private void OnClickGiveUpButton()
    {
        Debug.Log("OnClickGiveUpButton");

        // TODO : 게임 로비로 나가기
    }
}
