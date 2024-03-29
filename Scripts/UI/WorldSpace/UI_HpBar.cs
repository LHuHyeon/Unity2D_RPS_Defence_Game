using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_HpBar.cs
 * Desc :   "EnemyController"의 하위 항목으로 사용
 *          몬스터의 체력을 표시한다.
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : RefreshUI()   - UI 새로고침
 *
 */
 
public class UI_HpBar : UI_Base
{
    enum GameObjects
    {
        HpSlider,
    }

    private float           _posY = 0;   // 체력바 높이

    private EnemyStat       _stat;

    private Slider          _hpSlider;
    private Transform       _parent;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        _hpSlider = GetObject((int)GameObjects.HpSlider).GetComponent<Slider>();
        
        _parent = transform.parent;

        _stat   = _parent.GetComponent<EnemyStat>();

        _posY   = (_parent.GetComponent<Collider>().bounds.size.y + _parent.GetComponent<Collider>().bounds.size.y / 5);

        // 체력바 위치 설정
        transform.position = _parent.position + Vector3.up * _posY;
        
        RefreshUI();

        // 몬스터가 보스인지 확인
        if (_parent.GetComponent<EnemyController>()._isBoss == true)
            _parent.localScale *= 2;

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        float ratio = 0;

        // 방어력 or 체력에 따른 색 변경
        if (_stat.Shield > 0)
        {
            _hpSlider.fillRect.GetComponent<Image>().color = Color.gray;
            ratio = (float)_stat.Shield / _stat.MaxShield;
        }
        else
        {
            _hpSlider.fillRect.GetComponent<Image>().color = Color.red;
            ratio = (float)_stat.Hp / _stat.MaxHp;
        }
        
        if (float.IsNaN(ratio) == true)
            _hpSlider.value = 0;
        else
            _hpSlider.value = ratio;
    }
}
