using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool             IsBuildTower { set; get; }

    void Start()
    {
        IsBuildTower = false;
    }
}
