using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explostion : MonoBehaviour
{
    [SerializeField]
    private float disableTime = 1f;

    void OnEnable()
    {
        StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(disableTime);

        Managers.Resource.Destroy(this.gameObject);
    }
}
