using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
