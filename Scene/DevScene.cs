using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DevScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init())
            return false;

        SceneType = Define.Scene.Dev;
        return true;
	}
}
