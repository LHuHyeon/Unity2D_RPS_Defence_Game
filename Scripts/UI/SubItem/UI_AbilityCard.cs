using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_AbilityCard.cs
 * Desc :   "UI_DrawAbilityPopup"의 하위 항목으로 사용
 *          능력 카드의 역할을 맡고 선택/취소를 설정한다.
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : RefreshUI()   - UI 새로고침
 &  : Clear()       - 초기화
 &
 &  [Private]
 &  : OnClickAbilityCard()  - 능력 카드 클릭(선택)
 &  : IsChoice()            - 선택/취소 설정
 &  : SetColor()            - 컬러 설정(투명도)
 *
 */
 
public class UI_AbilityCard : UI_Base
{
    enum Images
    {
        Icon,
        ChoiceFrame,
    }

    enum Texts
    {
        AbilityNameText,
        DescripitionText,
    }

    public AbilityData _ability;

    public int  _currentValue = 0;
    public bool _isChoice = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent((PointerEventData eventData) => {OnClickAbilityCard();});

        UI_DrawAbilityPopup drawAbilityPopup = Managers.UI.FindPopup<UI_DrawAbilityPopup>();
        drawAbilityPopup._onClickAbilityCard -= OnClickAbilityCard;
        drawAbilityPopup._onClickAbilityCard += OnClickAbilityCard;

        RefreshUI();

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        IsChoice(false);

        // 능력 랜덤 뽑기
        Define.AbilityType abilityType = (Define.AbilityType)Random.Range(1, ((int)Define.AbilityType.Max));
        _ability = Managers.Data.Abilities[abilityType];

        // 능력 값 랜덤 뽑기
        int value = _ability.values[Random.Range(0, _ability.values.Count)];
        _currentValue = value;

        string descripition = _ability.descripition.Replace("{value}", value.ToString());

        GetImage((int)Images.Icon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Ability/"+abilityType.ToString());

        GetText((int)Texts.AbilityNameText).text = _ability.name;
        GetText((int)Texts.DescripitionText).text = descripition;
    }

    private void OnClickAbilityCard(GameObject go)
    {
        if (go == this.gameObject)
            return;

        IsChoice(false);
    }

    private void OnClickAbilityCard()
    {
        Debug.Log("OnClickAbilityCard");

        IsChoice(true);

        Managers.UI.FindPopup<UI_DrawAbilityPopup>()?._onClickAbilityCard.Invoke(this.gameObject);
    }

    private void IsChoice(bool isChoice)
    {
        _isChoice = isChoice;
        
        if (_isChoice == true)
            SetColor(GetImage((int)Images.ChoiceFrame), 1f);
        else
            SetColor(GetImage((int)Images.ChoiceFrame), 100f/255f);
    }
    
    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    public void Clear()
    {
        
    }
}
