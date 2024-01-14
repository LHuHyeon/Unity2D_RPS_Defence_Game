using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using TMPro;
using UnityEditor;
using UnityEngine;

/*
 * File :   DevScene.cs
 * Desc :   Test Scene
 */
 
public class DevScene : BaseScene
{
    [HideInInspector] public string PlayerName { get; private set; }
    [HideInInspector] public List<CharacterResult> Characters { get; private set; }

    [SerializeField]
    private GetPlayerCombinedInfoRequestParams infoRequestParams;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Dev;

        Managers.UI.ShowSceneUI<UI_DevScene>();
        
        Debug.Log("DevScene Init");

        return true;
	}

    public void OnLoginButton()
    {
        Managers.PlayFab.GoogleLogin(()=>
        {
            CallCSharpExecuteFunction();
        });
    }

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
            FunctionName = "HelloWorld", // Azure 함수의 이름
            FunctionParameter = new Dictionary<string, object>() { { "inputValue", "Test" } },
            GeneratePlayStreamEvent = true
        },
        CallSuccess, CallError);
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
    }

    private void CallError(PlayFabError error)
    {
        Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
    }
}
