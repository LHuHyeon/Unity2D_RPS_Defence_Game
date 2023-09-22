using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public MercenaryStat _mercenary;

    public bool             IsBuildTower { set; get; }

    void Start()
    {
        IsBuildTower = false;
    }

    public void SetInfo(MercenaryStat mercenary)
    {
        _mercenary = mercenary;
    }
}
