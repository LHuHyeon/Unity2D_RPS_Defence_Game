using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

using GooglePlayGames;

public class DevScene : BaseScene
{
    public TextMeshProUGUI googleText;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Dev;

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        GoogleLogin();

        Debug.Log("DevScene Init");

        return true;
	}

    // 구글 로그인
    public void GoogleLogin()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated == true)
        {
            googleText.text = "이미 접속 중입니다.";
            return;
        }

        googleText.text = "구글 로그인 중...";

        // 구글 연동 진행
        Social.localUser.Authenticate((bool success) =>
        {
            string str = $"ID : {Social.localUser.id} \n Name : {Social.localUser.userName} \n 구글 로그인 성공!";

            if (success == true)
                googleText.text = str;
            else
                googleText.text = "구글 로그인 실패 ;(";
        });
    }
}
