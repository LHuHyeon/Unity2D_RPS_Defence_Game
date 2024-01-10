using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   Explostion.cs
 * Desc :   마법사의 공격으로 스플래쉬가 발생되고 1초 뒤 제거하는 기능이다.
 */

public class Explostion : MonoBehaviour
{
    [SerializeField]
    private float   _disableTime = 1f;

    void OnEnable()
    {
        StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(_disableTime);

        Managers.Resource.Destroy(this.gameObject);
    }
}
