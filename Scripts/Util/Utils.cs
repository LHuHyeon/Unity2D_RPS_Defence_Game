using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using static Define;

public class Utils
{
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        // recursive : 자기 자신의 자식 객체를 가져올지 판단
        if (recursive == false)
        {
            // go의 자식객체 수 만큼
            for(int i=0; i<go.transform.childCount; i++)
            {
                // 지정된 자식객체를 transform에 반환
                Transform transform = go.transform.GetChild(i);

                // string.IsNullOrEmpty = 빈문자열이면 true (null 또는 "")
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    // 해당 T(Button, Text, ...) 컴포넌트 반환
                    T component = transform.GetComponent<T>();
                    if (component.IsNull() == false)
                        return component;
                }
            }
        }
        else{
            // true일 경우 자식의 자식까지 다 가져온다.
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    // 숫자 단위 붙이기
    public static string GetNumberUnitText(int number)
    {
        if (number.ToString().Length <= 4)
            return (number == 0) ? "0" : GetCommaText(number);

        // 숫자 구성 단위
        string[] unit = new string[] { "", "K", "M", "G", "T", "P", "E", "Z"};

        // 3칸씩 숫자 자리 지정 
        string num = string.Format("{0:# ### ### ### ### ### ### ### ###}", number).TrimStart().Replace(" ", ",");
        string[] str = num.Split(',');

        int cnt = str.Length - 1;
        int strNum = Convert.ToInt32(str[0]);

        string result = "";
        // 두자리 수까진 소수점 붙이기
        if (strNum.ToString().Length <= 2 && cnt > 0)
            result = strNum + "." + str[1].Substring(0, 1) + unit[cnt];
        else
            result = strNum + unit[cnt];

        return result;
    }

    // 콤마(,) 붙이기
    public static string GetCommaText(int number)
    {
        return string.Format("{0:#,###}", number);
    }
}
