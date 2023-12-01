using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager
{
    private string _playFabId;

    // PlayFab Google 로그인
    public void GoogleLogin(Action onLoginSuccessAction)
    {
        // 구글 초기화
        GoogleInit();

        // 구글 연동 진행
        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                Debug.Log("Google Play 인증 실패!");
                return;
            }

            // 구글 서버 인증코드 가져오기
            string serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();

            // PlayFab 보낼 메시지 만들기
            var request = new LoginWithGoogleAccountRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,    // 구글 서버 인증 코드
                CreateAccount = true                // 해당 Id가 없을 때 PlayFab 계정을 자동 생성할지 여부
            };

            // PlayFab 로그인 요청 보내기
            PlayFabClientAPI.LoginWithGoogleAccount(request, 
            (result)=>
            {
                // PlayFab 로그인이 성공하면
                Debug.Log("PlayFab Google 로그인 성공!");
                _playFabId = result.PlayFabId;

                onLoginSuccessAction.Invoke();
            },
            OnLoginFailed);
        });
    }

    // Google Play 초기화
    private void GoogleInit()
    {
        // 구글(GPGS) 초기화 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // 로그 활성화/비활성화
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log("PlayFab 로그인 실패!");
        Debug.Log("PlayFab Error : " + error.ErrorMessage);
    }
}
