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
        _pwCheckInput.onValueChanged.AddListener(delegate{ PWDoubleCheck(_pwCheckInput.text); });

        // Button Event
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);
        GetObject((int)GameObjects.ExitBackground).BindEvent((PointerEventData eventData)=> { OnClickExitButton(); });

        RefreshUI();

        return true;
    }
    
    public void RefreshUI()
    {
        if (_init == false)
            return;

        _userNameInput.Select();
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
        {
            Debug.Log("원활하지 않아요 정보가!!");
            Debug.Log("user : " + _userNameSuccess + ", email : " + _emailSuccess + ", pw : " + _pwSuccess + ", pwCheck : " + _pwCheckInput);
            return;
        }

        // SetHelper(GetText((int)Texts.SignUpHelperText), Define.SignupFalseText, false);

        var request = new RegisterPlayFabUserRequest { Email = _emailInput.text, Password = _pwCheckInput.text, Username = _userNameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Debug.Log("회원가입 성공!"), (error) => Debug.Log("회원가입 실패!"));
    }

    // 닉네임 문자열 체크
    private void UserNameCheck(string str)
    {
        // 길이 체크
        if (str == null || str.Length > Define.MAX_NICKNAME_LENGTH || str.Length < Define.MIN_NICKNAME_LENGTH)
        {
            // SetHelper(GetText((int)Texts.UserNameHelperText), Define.LengthFalseText, false);
            _userNameSuccess = false;
            return;
        }

        // 올바른 문자열 체크
        Regex regex = new Regex(@"^[0-9a-zA-Z가-힣]{2,8}$");
        if (regex.IsMatch(str))
        {
            // SetHelper(GetText((int)Texts.UserNameHelperText), Define.NickNameTrueText, true);
            _userNameSuccess =  true;
            return;
        }

        // SetHelper(GetText((int)Texts.UserNameHelperText), Define.NickNameFalseText, false);
        _userNameSuccess =  false;
    }

    // Email 문자열 체크
    private void EmailCheck(string str)
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

    // Password 문자열 체크
    private void PWCheck(string str)
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
    private void PWDoubleCheck(string str)
    {
        if (_pwInput.text == _pwCheckInput.text)
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

    // Text 효과 적용
    private void SetHelper(TextMeshProUGUI text, int textId, bool success)
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
}
