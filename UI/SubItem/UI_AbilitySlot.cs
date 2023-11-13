using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AbilitySlot : UI_Base
{
    enum Images
    {
        AbilityIcon,
    }

    enum Texts
    {
        AbilityNameText,
        AbilityValueText,
    }

    private AbilityData _ability;

    private int         _currentValue = 0;
    private List<int>   _values;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        Clear();

        RefreshUI();

        return true;
    }

    public void SetInfo(AbilityData abilityData)
    {
        _ability = abilityData;

        RefreshUI();
    }

    public void RefreshDescripition(int value)
    {
        if (_init == false)
            return;

        if (value <= 0)
            return;
            
        _currentValue += value;
        _values.Add(value);

        GetText((int)Texts.AbilityValueText).text = $"{_currentValue}% ";

        if (_values.Count <= 1)
            return;

        // 어떤 값들이 합쳐졌는지 괄호 추가
        string valuesStr = "";
        for(int i=0; i<_values.Count; i++)
        {
            valuesStr += $" {_values[i]}%";
            
            if (i < _values.Count-1)
                valuesStr += " +";
        }

        GetText((int)Texts.AbilityValueText).text += $"( {valuesStr} )";
    }

    private void RefreshUI()
    {
        if (_init == false)
            return;

        GetImage((int)Images.AbilityIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Ability/"+_ability.abilityType.ToString());

        GetText((int)Texts.AbilityNameText).text = _ability.name;

        RefreshDescripition(_ability.currentValue);
    }

    public void Clear()
    {
        _currentValue = 0;
        _values = new List<int>();
    }
}
