using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * File :   UI_EvolutionText.cs
 * Desc :   "UI_MercenaryInfoPopup"의 하위 항목으로 사용
 *          진화 능력 정보를 표시하는데 사용한다.
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : SetInfo()     - 정보 설정
 &  : RefreshUI()   - UI 새로고침
 &
 &  [Private]
 &  : PopulateStarIcon()    - ★ Icon 채우기
 &  : SetColor()            - 컬러 설정(투명도)
 *
 */
 
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

    public Define.EvolutionType _evolutionType = Define.EvolutionType.Unknown;

    private Define.EvolutionType _currentEvolution;

    private BuffData _buff;

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

    public void SetInfo(BuffData buff, Define.EvolutionType evolutionType)
    {
        _buff = buff;
        _currentEvolution = evolutionType;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        GetText((int)Texts.EvolutionText).text = _buff.descripition;
        SetColor(GetText((int)Texts.EvolutionText), (_currentEvolution >= _evolutionType ? 1f : 0.5f));
    }

    private void PopulateStarIcon()
    {
        int currentStarCount = 0;

        foreach(Transform child in GetObject((int)GameObjects.StarGrid).transform)
        {
            currentStarCount++;

            Image icon = child.GetComponent<Image>();

            if (currentStarCount <= ((int)_evolutionType))
                icon.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_Star");
            else
                icon.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Icon_Evolution_DeStar");
        }
    }

    private void SetColor(TextMeshProUGUI text, float alpha)
    {
        Color _color = text.color;
        _color.a = alpha;
        text.color = _color;
    }
}
