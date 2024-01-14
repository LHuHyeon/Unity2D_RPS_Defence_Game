using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadScene : UI_Scene
{
    enum Texts
    {
        HelperText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));

        StartCoroutine(NetworkCheck());

        return true;
    }

    // 네트워크 확인
    private IEnumerator NetworkCheck()
    {
        GetText((int)Texts.HelperText).text = "네트워크 연결 확인...";

        while (true)
        {
            // 인터넷 연결이 안되었을 때
            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            StartCoroutine(OnSceneCoroutine());
            break;
        }
    }

    // 기본 데이터를 다 가져오면 Scene 코드 진행
    private IEnumerator OnSceneCoroutine()
    {
        GetText((int)Texts.HelperText).text = "데이터 가져오는 중...";

        // 데이터 가져오기
		while(Managers.Data.IsData() == false)
			yield return new WaitForSeconds(0.1f);

        GetText((int)Texts.HelperText).text = "게임 연결 중...";

        // Game Scene 비동기 연결 시작
        AsyncOperation operation = Managers.Scene.LoadAsynScene(Define.Scene.Game);

        // 2초 뒤 게임 Scene 전환
        yield return new WaitForSeconds(2f);
        
        operation.allowSceneActivation = true;
    }
}
