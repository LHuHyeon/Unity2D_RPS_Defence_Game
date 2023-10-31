using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EvolutionBar : UI_Base
{
    enum GameObjects
    {
        StarGrid,
    }

    private MercenaryStat       _stat;
    private List<GameObject>    _stars = new List<GameObject>();
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
        {
            _stars.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        RefreshUI();

        return true;
    }

    public void SetInfo(MercenaryStat stat)
    {
        _stat = stat;

        transform.localPosition = Vector3.up * 0.015f;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        for(int i=0; i<((int)_stat.CurrentEvolution); i++)
            _stars[i].SetActive(true);
    }
}