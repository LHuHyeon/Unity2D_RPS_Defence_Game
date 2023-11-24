using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using PlayFab;
using PlayFab.ClientModels;

public class UI_SignUpPopup : UI_Popup
{
    enum GameObjects
    {
        ExitBackground,
    }

    enum Inputs
    {
        UserNameInput,
        EmailInput,
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
        TitleText,
        SignUpButtonText,
        UserNameHelperText,
        EmailHelperText,
        PWHelperText,
        PWCheckHelperText,
        SignUpHelperText,
    }

    // 회원가입 정보 입력 여부
    private bool _userNameSuccess   = false;
    private bool _emailSuccess      = false;
    private bool _pwSuccess         = false;
    private bool _pwCheckSuccess    = false;

    private TMP_InputField _userNameInput;
    private TMP_InputField _emailInput;
    private TMP_InputField _pwInput;
    private TMP_InputField _pwCheckInput;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindInput(typeof(Inputs));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        
        // Button
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);
        GetObject((int)GameObjects.ExitBackground).BindEvent((PointerEventData eventData)=> { OnClickExitButton(); });

        // Text
        GetText((int)Texts.TitleText).text = Managers.GetText(Define.SignUp);
        GetText((int)Texts.SignUpButtonText).text = Managers.GetText(Define.SignUp);

        // InputField 변수에 담기
        _userNameInput  = GetInput((int)Inputs.UserNameInput);
        _emailInput     = GetInput((int)Inputs.EmailInput);
        _pwInput        = GetInput((int)Inputs.PWInput);
        _pwCheckInput   = GetInput((int)Inputs.PWCheckInput);

        // InputField 다음으로 넘기기
        _userNameInput.onSubmit.AddListener(delegate{ _emailInput.Select(); });
        _emailInput.onSubmit.AddListener(delegate{ _pwInput.Select(); });
        _pwInput.onSubmit.AddListener(delegate{ _pwCheckInput.Select(); });

        // InputField 문자열 체크 이벤트
        _userNameInput.onValueChanged.AddListener(delegate{ UserNameCheck(_userNameInput.text); });
        _emailInput.onValueChanged.AddListener(delegate{ EmailCheck(_emailInput.text); });
        _pwInput.onValueChanged.AddListener(delegate{ PWCheck(_pwInput.text); });
        _pwCheckInput.onValueChanged.AddListener(delegate{ PWDoubleCheck(); });

        RefreshUI();

        return true;
    }
    
    public void RefreshUI()
    {
        if (_init == false)
            return;

        _userNameInput.Select();

        GetText((int)Texts.UserNameHelperText).text = "";
        GetText((int)Texts.EmailHelperText).text = "";
        GetText((int)Texts.PWHelperText).text = "";
        GetText((int)Texts.PWCheckHelperText).text = "";
        GetText((int)Texts.SignUpHelperText).text = "";
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
    private void OnSignup()
    {
        // 입력한 회원가입 정보 확인
        if (!_userNameSuccess || !_emailSuccess || !_pwSuccess || !_pwCheckSuccess)
            return;

        var request = new RegisterPlayFabUserRequest { Email = _emailInput.text, Password = _pwCheckInput.text, Username = _userNameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailed);

        // TODO : 닉네임, 이메일 중복을 확인하여 재입력 및 경고 표시해주기

        // SetHelper(GetText((int)Texts.SignUpHelperText), Define.SignupFalseText, false);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("회원가입 성공!");
    }

    private void OnRegisterFailed(PlayFabError error)
    {
        Debug.Log("회원가입 실패!");
    }

    // 닉네임 문자열 체크
    private void UserNameCheck(string str)
    {
        // 길이와 올바른 문자열 체크
        Regex regex = new Regex(@"^[0-9a-zA-Z]{2,8}$");
        if (regex.IsMatch(str))
        {
            SetHelper(GetText((int)Texts.UserNameHelperText), true);
            _userNameSuccess =  true;
            return;
        }

        SetHelper(GetText((int)Texts.UserNameHelperText), false, Define.UserNameLength);
        _userNameSuccess =  false;
    }

    // Email 문자열 체크
    private void EmailCheck(string str)
    {
        Regex regex = new Regex(@"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$");
        if (regex.IsMatch(str))
        {
            SetHelper(GetText((int)Texts.EmailHelperText), true);
            _emailSuccess = true;
            return;
        }
        
        SetHelper(GetText((int)Texts.EmailHelperText), false, Define.EmailCheckFalse);
        _emailSuccess = false;
    }

    // Password 문자열 체크
    private void PWCheck(string str)
    {
        if (str.IsNull() == true)
            return;

        if (str != null && str.Length <= Define.MAX_PASSWORD_LENGTH && str.Length >= Define.MIN_PASSWORD_LENGTH)
        {
            SetHelper(GetText((int)Texts.PWHelperText), true);
            _pwSuccess = true;
            return;
        }

        SetHelper(GetText((int)Texts.PWHelperText), false, Define.PasswordLength);
        _pwSuccess = false;
    }

    // Password 한번더 체크
    private void PWDoubleCheck()
    {
        if (_pwInput.text == _pwCheckInput.text)
        {
            SetHelper(GetText((int)Texts.PWCheckHelperText), true);
            _pwCheckSuccess = true;
        }
        else
        {
            SetHelper(GetText((int)Texts.PWCheckHelperText), false, Define.PasswordCheckFalse);
            _pwCheckSuccess = false;
        }
    }

    // Text 효과 적용
    private void SetHelper(TextMeshProUGUI text, bool success, int textId = 0)
    {
        if (success == true)
        {
            text.text = "";
            return;
        }

        text.text = Managers.GetText(textId);
        text.color = Color.red;
    }

    public void Clear()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
