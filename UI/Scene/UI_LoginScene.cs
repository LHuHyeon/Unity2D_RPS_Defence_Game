using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;

public class UI_LoginScene : UI_Scene
{
    enum Inputs
    {
        EmailInput,
        PWInput,
    }

    enum Buttons
    {
        LoginButton,
        SignUpButton,
        GoogleButton,
    }

    enum Texts
    {
        LoginButtonText,
        SignUpButtonText,
        GoogleButtonText,
        LoginHelperText,
    }

    private TMP_InputField _emailInput;
    private TMP_InputField _pwInput;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindInput(typeof(Inputs));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        _emailInput = GetInput((int)Inputs.EmailInput);
        _pwInput = GetInput((int)Inputs.PWInput);

        // Button
        GetButton((int)Buttons.LoginButton).onClick.AddListener(OnClickLoginButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);
        GetButton((int)Buttons.GoogleButton).onClick.AddListener(OnClickGoogleLoginButton);

        // Text
        GetText((int)Texts.LoginButtonText).text = Managers.GetText(Define.Login);
        GetText((int)Texts.SignUpButtonText).text = Managers.GetText(Define.SignUp);
        GetText((int)Texts.GoogleButtonText).text = Managers.GetText(Define.GoogleLogin);
        GetText((int)Texts.LoginHelperText).text = "";

        // InputField
        _emailInput.onSubmit.AddListener(delegate{ _pwInput.Select(); });
        _pwInput.onSubmit.AddListener(delegate{ OnClickLoginButton(); });

        return true;
    }

    // 로그인 버튼
    private void OnClickLoginButton()
    {
        Debug.Log("OnClickLoginButton");

        OnLogin();
    }

    // 회원가입 버튼
    private void OnClickSignUpButton()
    {
        Debug.Log("OnClickSignUpButton");

        Managers.UI.ShowPopupUI<UI_SignUpPopup>().RefreshUI();
    }

    // 구글 로그인 버튼
    private void OnClickGoogleLoginButton()
    {
        Debug.Log("OnClickGoogleButton");

        // 구글 로그인 진행
        Managers.PlayFab.GoogleLogin(()=>
        {
            // Loby Scene Load
            Managers.Scene.LoadScene(Define.Scene.Loby);
        });
    }

    private void OnLogin()
    {
        if (_init == false)
            return;

        // ID와 Password 올바른 문자열 확인
        if (EmailCheck(_emailInput.text) == false || PasswordCheck(_pwInput.text) == false)
        {
            GetText((int)Texts.LoginHelperText).text = Managers.GetText(Define.LoginFalse);
            return;
        }

        // TODO : Login 정보가 서버에 저장되어 있는지 확인 후 Loby로 접속
        var request = new LoginWithEmailAddressRequest { Email = _emailInput.text, Password = _pwInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailed);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공!");

        Managers.Scene.LoadScene(Define.Scene.Loby);
    }

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log("로그인 실패!");
    }
    
    // Email 문자열 체크
    private bool EmailCheck(string str)
    {
        if (str.IsNull() == true)
            return false;

        Regex regex = new Regex(Define.RegexEmail);
        if (regex.IsMatch(str))
        {
            Debug.Log("이메일 문자열 통과");
            return true;
        }

        Debug.Log("EmailCheck : 불완전한 이메일입니다.");

        return false;
    }

    // Password 문자열 체크
    private bool PasswordCheck(string str)
    {
        if (str.IsNull() == true)
            return false;

        if (str.Length <= Define.MAX_PASSWORD_LENGTH && str.Length >= Define.MIN_PASSWORD_LENGTH)
        {
            Debug.Log("비밀번호 문자열 통과");
            return true;
        }

        return false;
    }
}
