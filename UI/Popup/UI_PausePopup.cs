using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_PausePopup.cs
 * Desc :   설정 Popup
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : SetInfo()     - 정보 설정
 &  : RefreshUI()   - UI 새로고침
 &  : Clear()       - 초기화
 &
 &  [private]
 &  : OnClickSettingButton()    - 설정 버튼
 &  : OnClickContinueButton()   - 게임 진행 버튼
 &  : OnClickGiveUpButton()     - 나가기 버튼
 *
 */
 
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

    private float   _currentGameSpeed;
    private bool    _isActive = false;

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

        if (_isActive == false)
            StartCoroutine(CallPopup());
    }

    private void OnClickSettingButton()
    {
        Debug.Log("OnClickSettingButton");

        // TODO : 설정 구현
    }
    
    private void OnClickContinueButton()
    {
        Debug.Log("OnClickContinueButton");

        StartCoroutine(ExitPopup());
    }

    private void OnClickGiveUpButton()
    {
        Debug.Log("OnClickGiveUpButton");

        Application.Quit();

        // TODO : 게임 로비로 나가기
    }

    private float maxAlpha = 220f/255f;  // 투명도 최대치
    private IEnumerator CallPopup()
    {
        _isActive = true;

        // 배경 가져오기
        Transform background = GetObject((int)GameObjects.ExitBackground).transform;
        Image icon = background.GetComponent<Image>();

        SetColor(icon, 0);

        // 배경 어둡게
        float currentAlpha = 0f;
        while (currentAlpha < maxAlpha)
        {
            yield return null;

            currentAlpha += 0.1f;
            SetColor(icon, currentAlpha);
        }
    }

    private IEnumerator ExitPopup()
    {
        // 배경 가져오기
        Transform background = GetObject((int)GameObjects.ExitBackground).transform;
        Image icon = background.GetComponent<Image>();

        // 배경 어둡게
        float currentAlpha = maxAlpha;
        while (currentAlpha > 0)
        {
            yield return null;

            currentAlpha -= 0.1f;
            SetColor(icon, currentAlpha);
        }

        Clear();
    }

    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    public void Clear()
    {
        _isActive = false;
        Time.timeScale = _currentGameSpeed;
        Managers.UI.ClosePopupUI(this);
    }
}
