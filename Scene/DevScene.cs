using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.ClientModels;

public class DevScene : BaseScene
{
    public TextMeshProUGUI googleText;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Dev;

        // 구글(GPGS) 초기화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Debug.Log("DevScene Init");

        return true;
	}

    public void OnLoginButton()
    {
        GoogleLogin();
    }

    // 구글 연동 후 PlayFab에 전송
    private void GoogleLogin()
    {
        // 구글 연동 진행
        Social.localUser.Authenticate((bool success) =>
        {
            googleText.text = "Google Signed In";

            if (!success)
            {
                Debug.Log("구글 사용자 인증 실패!");
                googleText.text = "Google Failed to Authorize your login";
                return;
            }

            // 구글 서버 인증 코드 가져오기
            string serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();

            // PlayFab 보낼 메시지 만들기
            var request = new LoginWithGoogleAccountRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,    // 구글 서버 인증 코드
                CreateAccount = true                // 해당 Id가 없을 때 PlayFab 계정을 자동 생성할지 여부
            };

            // PlayFab 로그인 요청 보내기
            PlayFabClientAPI.LoginWithGoogleAccount(request, OnLoginSuccess, OnLoginFailed);
            
            Debug.Log("Server Auth Code: " + serverAuthCode);
            Debug.Log("Title Id : " + PlayFabSettings.TitleId);
        });
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공!");

        googleText.text = "Signed In as " + result.PlayFabId;
    }

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log("로그인 실패!");
        Debug.Log(error.ErrorMessage);

        googleText.text = "PlayFab Login Failed";
    }
}
