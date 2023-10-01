using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool             IsBuildTower { set; get; }

    public GameObject       mercenaryObj;

    void Start()
    {
        IsBuildTower = false;
    }
}
