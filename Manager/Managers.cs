using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    private static Managers s_instance = null;
    public static Managers Instance { get { return s_instance; } }

    private static DataManager s_dataManager;
    private static GameManagerEx s_gameManager = new GameManagerEx();
    private static UIManager s_uiManager = new UIManager();
    private static PoolManager s_poolManager = new PoolManager();
    private static ResourceManager s_resourceManager = new ResourceManager();
    private static SceneManagerEx s_sceneManager = new SceneManagerEx();
    private static SoundManager s_soundManager = new SoundManager();

    public static DataManager Data { get { Init(); return s_dataManager; } }
    public static GameManagerEx Game { get { Init(); return s_gameManager; } }
    public static UIManager UI { get { Init(); return s_uiManager; } }
    public static PoolManager Pool { get { Init(); return s_poolManager; } }
    public static ResourceManager Resource { get { Init(); return s_resourceManager; } }
    public static SceneManagerEx Scene { get { Init(); return s_sceneManager; } }
    public static SoundManager Sound {  get { Init(); return s_soundManager; } }

    public static string GetText(int id)
	{
        if (Managers.Data.Texts.TryGetValue(id, out TextData value) == false)
            return "";

        return value.kor;
	}

    private void Start()
    {
        Init();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            s_instance = CreateMonoBehaviour<Managers>("@Managers");
            s_dataManager = CreateMonoBehaviour<DataManager>("@DataManager");

            s_dataManager.Init();
            s_resourceManager.Init();
            s_soundManager.Init();
            
            Application.targetFrameRate = 60;
        }
    }

    // MonoBehaviour이 필요한 Manager가 사용
    private static T CreateMonoBehaviour<T>(string objectName) where T : MonoBehaviour
    {
        GameObject go = GameObject.Find(objectName);
        if (go == null)
            go = new GameObject { name = objectName };

        DontDestroyOnLoad(go);

        return Utils.GetOrAddComponent<T>(go);
    }

    public static void Clear()
    {
        s_uiManager.Clear();
    }
}
