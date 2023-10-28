using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Base
{
    enum GameObjects
    {
        HpSlider,
    }

    private float           _posY = 0;   // 체력바 높이

    private EnemyStat       _stat;
    private EnemyController _enemy;

    private Slider          hpSlider;
    private Transform       parent;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        hpSlider = GetObject((int)GameObjects.HpSlider).GetComponent<Slider>();
        
        parent  = transform.parent;

        _stat   = parent.GetComponent<EnemyStat>();
        _enemy  = parent.GetComponent<EnemyController>();

        _posY = (parent.GetComponent<Collider>().bounds.size.y + parent.GetComponent<Collider>().bounds.size.y / 5);

        RefreshUI();

        return true;
    }

    void FixedUpdate()
    {
        // 체력바 위치 설정
        transform.position = parent.position + Vector3.up * _posY;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        float ratio = 0;

        // 방어력 or 체력에 따른 색 변경
        if (_stat.Shield > 0)
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.gray;
            ratio = (float)_stat.Defence / _stat.MaxDefence;
        }
        else
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
            ratio = (float)_stat.Hp / _stat.MaxHp;
        }
        
        hpSlider.value = ratio;
    }
}
