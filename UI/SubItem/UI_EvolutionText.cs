using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EvolutionText : UI_Base
{
    enum GameObjects
    {
        StarGrid,
    }

    enum Texts
    {
        EvolutionText,
    }

    public int          _starCount = 0;

    private AbilityData _ability;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        PopulateStarIcon();

        RefreshUI();

        return true;
    }

    public void SetInfo(AbilityData ability)
    {
        _ability = ability;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        GetText((int)Texts.EvolutionText).text = _ability.descripition;
    }

    private void PopulateStarIcon()
    {
        int currentStarCount = 0;

        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
        {
            currentStarCount++;

            Image icon = child.GetComponent<Image>();

            if (currentStarCount <= _starCount)
                icon.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_Star");
            else
                icon.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_DeStar");
        }
    }
}
