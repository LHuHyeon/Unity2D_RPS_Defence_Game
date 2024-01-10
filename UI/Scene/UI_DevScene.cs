using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine.Networking;
using System.Text;

/*
 * File :   UI_DevScene.cs
 * Desc :   Test Scene
 */

public class UI_DevScene : UI_Scene
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
    }

    enum Texts
    {
        LoginText,
    }

    private TMP_InputField _emailInput;
    private TMP_InputField _pwInput;

    private string tempEmail    = "rpstest@gmail.com";
    private string tempPW       = "rpstest123";

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindInput(typeof(Inputs));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        _emailInput = GetInput((int)Inputs.EmailInput);
        _pwInput = GetInput((int)Inputs.PWInput);

        _emailInput.onSubmit.AddListener(delegate{ _pwInput.Select(); });

        GetButton((int)Buttons.LoginButton).onClick.AddListener(OnClickLoginButton);
        GetButton((int)Buttons.SignUpButton).onClick.AddListener(OnClickSignUpButton);

        return true;
    }

    private void OnClickLoginButton()
    {
        // var request = new LoginWithEmailAddressRequest { Email = _emailInput.text, Password = _pwInput.text };
        var request = new LoginWithEmailAddressRequest { Email = tempEmail, Password = tempPW };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailed);
    }

    private void OnClickSignUpButton()
    {
        Managers.UI.ShowPopupUI<UI_SignUpPopup>();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공!");
        Debug.Log("PlayFab Id : " + result.PlayFabId);

        GetText((int)Texts.LoginText).text = "로그인 성공!";

        RegisterUserRequest();
    }

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log("로그인 실패!");
        Debug.Log(error.ErrorMessage);

        GetText((int)Texts.LoginText).text = "로그인 실패!";
    }

    // 유저 정보 통계에 받기
    private void RegisterUserRequest()
    {
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        { 
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                // 로그인할 때부터 가져오기 ( Id, Type )
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType
            },
            FunctionName = "RegisterUserRequest", // Azure 함수의 이름
            GeneratePlayStreamEvent = true
        },
        CallSuccess, CallError);
    }

    // TODO : Test 중.. 뭔지 모르겠지만 아무리 시도해도 안된다..
    private void CallCSharpExecuteFunction()
    {
        // Azure Cloud Script 함수 실행
        PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        { 
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                // 로그인할 때부터 가져오기 ( Id, Type )
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType
            },
            FunctionName = "HelloRPS", // Azure 함수의 이름
            FunctionParameter = new Dictionary<string, object>() { { "inputValue", "TestTestTest" } },
            GeneratePlayStreamEvent = true
        },
        CallSuccess, CallError);

        //// 로컬 구현 테스트
        // PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
        // {
        //     FunctionName = "ExecuteFunction", // Azure 함수의 이름
        //     Entity = new PlayFab.CloudScriptModels.EntityKey()
        //     {
        //         // 로그인할 때부터 가져오기 ( Id, Type )
        //         Id = PlayFabSettings.staticPlayer.EntityId,
        //         Type = PlayFabSettings.staticPlayer.EntityType
        //     },
        //     FunctionParameter = new Dictionary<string, object>()
        //     {
        //         { "X-EntityToken", PlayFabSettings.staticPlayer.EntityToken }
        //     },
        //     GeneratePlayStreamEvent = false,
        // }, CallSuccess, CallError);
    }

    private void CallSuccess(ExecuteFunctionResult result)
    {
        if (result.FunctionResultTooLarge != null && (bool)result.FunctionResultTooLarge)
        {
            Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function," +
                " See PlayFab Limits Page for details.");
            return;
        }
        Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
        Debug.Log($"Result: {result.FunctionResult.ToString()}");
        Debug.Log("클라우드 성공!");
    }

    private void CallError(PlayFabError error)
    {
        Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
        Debug.Log($"Function execution failed. Error: {error.ErrorMessage}");
    }
}
