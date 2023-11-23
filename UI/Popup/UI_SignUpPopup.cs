using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class UI_SignUpPopup : UI_Popup
{
    enum GameObjects
    {
        ExitBackground,
    }

    enum Inputs
    {
        EmailInput,
        IDInput,
        PWInput,
        PWCheckInput,
    }

    enum Buttons
    {
        ExitButton,
        SignUpButton,
    }

    enum Texts
    {
        EmailHelperText,
        IDHelperText,
        PWHelperText,
        PWCheckHelperText,
        SignUpHelperText,
    }

    // 회원가입 정보 입력 여부
    bool _IdSuccess         = false;
    bool _pwSuccess         = false;
    bool _pwCheckSuccess    = false;
    bool _emailSuccess      = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindInput(typeof(Inputs));
        BindButton(typeof(Buttons));

        // InputField 다음으로 넘기기
        GetInput((int)Inputs.EmailInput).onSubmit.AddListener(delegate{ GetInput((int)Inputs.IDInput).Select(); });
        GetInput((int)Inputs.IDInput).onSubmit.AddListener(delegate{ GetInput((int)Inputs.PWInput).Select(); });
        GetInput((int)Inputs.PWInput).onSubmit.AddListener(delegate{ GetInput((int)Inputs.PWCheckInput).Select(); });

        // InputField 문자열 체크 이벤트
        GetInput((int)Inputs.EmailInput).onValueChanged.AddListener(delegate{ EmailCheck(GetInput((int)Inputs.EmailInput).text); });
        GetInput((int)Inputs.IDInput).onValueChanged.AddListener(delegate{ IdCheck(GetInput((int)Inputs.IDInput).text); });
        GetInput((int)Inputs.PWInput).onValueChanged.AddListener(delegate{ PWCheck(GetInput((int)Inputs.PWInput).text); });
        GetInput((int)Inputs.PWCheckInput).onValueChanged.AddListener(delegate{ PWDoubleCheck(GetInput((int)Inputs.PWCheckInput).text); });

        // Button
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);

        return true;
    }
    
    public void RefreshUI()
    {
        if (_init == false)
            return;
    }

    private void OnClickExitButton()
    {
        Debug.Log("OnClickExitButton");

        Clear();
    }

    private void OnClickSignUpButton()
    {
        Debug.Log("OnClickSignUpButton");

        OnSignup();
    }

    // 회원가입 확인 버튼
    void OnSignup()
    {
        // 입력한 회원가입 정보 확인
        if (!_IdSuccess || !_pwSuccess || !_pwCheckSuccess || !_emailSuccess)
            return;

        // SetHelper(GetText((int)Texts.SignUpHelperText), Define.SignupFalseText, false);
    }

    // ID 문자열 체크
    void IdCheck(string str)
    {
        if (str != null && str.Length <= Define.MAX_LOGIN_LENGTH && str.Length >= Define.MIN_LOGIN_LENGTH)
        {
            Regex regex = new Regex(@"^[0-9a-zA-Z]{4,12}$");
            if (regex.IsMatch(str))
            {
                // SetHelper(GetText((int)Texts.IDHelperText), Define.IdTrueText, true);
                _IdSuccess = true;
                return;
            }
        }

        // SetHelper(GetText((int)Texts.IDHelperText), Define.IdFalseText, false);
        _IdSuccess = false;
    }

    // Password 문자열 체크
    void PWCheck(string str)
    {
        if (str != null && str.Length <= Define.MAX_PASSWORD_LENGTH && str.Length >= Define.MIN_PASSWORD_LENGTH)
        {
            Regex regex = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,18}$", RegexOptions.IgnorePatternWhitespace);
            if (regex.IsMatch(str))
            {
                // SetHelper(GetText((int)Texts.PWHelperText), Define.PasswordTrueText, true);
                _pwSuccess = true;
                return;
            }
        }

        // SetHelper(GetText((int)Texts.PWHelperText), Define.PasswordFalseText, false);
        _pwSuccess = false;
    }

    // Password 한번더 체크
    void PWDoubleCheck(string str)
    {
        if (GetInput((int)Inputs.PWInput).text == GetInput((int)Inputs.PWCheckInput).text)
        {
            // SetHelper(GetText((int)Texts.PWCheckHelperText), Define.PasswordCheckTrueText, true);
            _pwCheckSuccess = true;
        }
        else
        {
            // SetHelper(GetText((int)Texts.PWCheckHelperText), Define.PasswordCheckFalseText, false);
            _pwCheckSuccess = false;
        }
    }

    // Email 문자열 체크
    void EmailCheck(string str)
    {
        Regex regex = new Regex(@"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$");
        if (regex.IsMatch(str))
        {
            // SetHelper(GetText((int)Texts.EmailHelperText), Define.EmailTrueText, true);
            _emailSuccess = true;
            return;
        }
        
        // SetHelper(GetText((int)Texts.EmailHelperText), Define.EmailFalseText, false);
        _emailSuccess = false;
    }

    // Text 효과 적용
    public void SetHelper(TextMeshProUGUI text, int textId, bool success)
    {
        text.text = Managers.GetText(textId);
        if (success == true)
            text.color = Color.green;
        else
            text.color = Color.red;
    }

    public void Clear()
    {
        Managers.UI.ClosePopupUI(this);
    }

    // 닉네임 문자열 체크
    // bool NickNameCheck(string str)
    // {
    //     // 길이 체크
    //     if (str == null || str.Length > Define.MAX_NICKNAME_LENGTH || str.Length < Define.MIN_NICKNAME_LENGTH)
    //     {
    //         SetHelper(GetText((int)Texts.CheckHelper), Define.LengthFalseText, false);
    //         return false;
    //     }

    //     // 올바른 문자열 체크
    //     Regex regex = new Regex(@"^[0-9a-zA-Z가-힣]{2,8}$");
    //     if (regex.IsMatch(str))
    //     {
    //         SetHelper(GetText((int)Texts.CheckHelper), Define.NickNameTrueText, true);
    //         return true;
    //     }

    //     SetHelper(GetText((int)Texts.CheckHelper), Define.NickNameFalseText, false);
    //     return false;
    // }
}
