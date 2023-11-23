using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class UI_LoginScene : UI_Scene
{
    enum Inputs
    {
        IDInput,
        PWInput,
    }

    enum Buttons
    {
        LoginButton,
        SignUpButton,
        GoogleButton,
    }

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

        // Input Field
        GetInput((int)Inputs.IDInput).onSubmit.AddListener(delegate{ GetInput((int)Inputs.PWInput).Select(); });
        GetInput((int)Inputs.PWInput).onSubmit.AddListener(delegate{ OnClickLoginButton(); });

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

        Managers.UI.ShowPopupUI<UI_SignUpPopup>();
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
        if (IdCheck(GetInput((int)Inputs.IDInput).text) == false || PasswordCheck(GetInput((int)Inputs.PWInput).text) == false)
            return;

        // TODO : Login 정보가 서버에 저장되어 있는지 확인 후 Loby로 접속
    }
    
    // ID 문자열 체크
    private bool IdCheck(string str)
    {
        if (str.IsNull() == true)
            return false;

        if (str.Length <= Define.MAX_LOGIN_LENGTH && str.Length >= Define.MIN_LOGIN_LENGTH)
        {
            Regex regex = new Regex(@"^[0-9a-zA-Z]{4,12}$");
            if (regex.IsMatch(str))
            {
                Debug.Log("아이디 문자열 통과");
                return true;
            }

            Debug.Log("IDPassCheck : 특수문자는 금지입니다.");
        }

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
