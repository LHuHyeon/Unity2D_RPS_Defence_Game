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

    private TMP_InputField _emailInput;
    private TMP_InputField _pwInput;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindInput(typeof(Inputs));
        BindButton(typeof(Buttons));

        // Button
        GetButton((int)Buttons.LoginButton).onClick.AddListener(OnClickLoginButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);
        GetButton((int)Buttons.GoogleButton).onClick.AddListener(OnClickGoogleButton);

        _emailInput = GetInput((int)Inputs.EmailInput);
        _pwInput = GetInput((int)Inputs.PWInput);

        // InputField
        _emailInput.onSubmit.AddListener(delegate{ _pwInput.Select(); });
        _pwInput.onSubmit.AddListener(delegate{ OnClickLoginButton(); });

        return true;
    }

    private void OnClickLoginButton()
    {
        Debug.Log("OnClickLoginButton");

        OnLogin();
    }

    private void OnClickSignUpButton()
    {
        Debug.Log("OnClickSignUpButton");

        Managers.UI.ShowPopupUI<UI_SignUpPopup>().RefreshUI();
    }

    private void OnClickGoogleButton()
    {
        Debug.Log("OnClickGoogleButton");

        // TODO : 구글 로그인
    }

    private void OnLogin()
    {
        if (_init == false)
            return;

        // ID와 Password 올바른 문자열 확인
        if (EmailCheck(_emailInput.text) == false || PasswordCheck(_pwInput.text) == false)
            return;

        // TODO : Login 정보가 서버에 저장되어 있는지 확인 후 Loby로 접속
        var request = new LoginWithEmailAddressRequest { Email = _emailInput.text, Password = _pwInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => Debug.Log("로그인 성공!"), (error) => Debug.Log("로그인 실패!"));
    }
    
    // Email 문자열 체크
    private bool EmailCheck(string str)
    {
        if (str.IsNull() == true)
            return false;

        Regex regex = new Regex(@"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$");
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
            Regex regex = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,18}$", RegexOptions.IgnorePatternWhitespace);
            if (regex.IsMatch(str))
            {
                Debug.Log("비밀번호 문자열 통과");
                return true;
            }

            Debug.Log("PasswordCheck : 영문, 숫자, 특수기호가 포함되어야 합니다.");
        }

        return false;
    }
}
