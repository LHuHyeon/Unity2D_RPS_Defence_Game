using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Extension
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Utils.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    // 스크롤 리셋 (0 : 시작 위치, 1 : 끝 위치)
	public static void ResetVertical(this ScrollRect scrollRect, int movePos)    { scrollRect.verticalNormalizedPosition = movePos; }
	public static void ResetHorizontal(this ScrollRect scrollRect, int movePos)  { scrollRect.horizontalNormalizedPosition = movePos; }

	// 참조형식(Reference) null 체크
    public static bool IsNull(this UnityEngine.Object go)   { return ReferenceEquals(go, null); }
    public static bool IsNull(this System.Object go)        { return ReferenceEquals(go, null); }

    // Fake Null 체크
    public static bool IsFakeNull(this UnityEngine.Object go) { return (go.IsNull() == false && go == true) == false; }

    // 객체 유효성 확인
    public static bool isValid(this GameObject go) { return go.IsNull() == false && go.activeSelf == true; }
}