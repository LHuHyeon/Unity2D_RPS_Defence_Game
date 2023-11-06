using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        // TODO : 능력 카드 새로고침
    }

    private void OnClickAbilityCard(GameObject go)
    {
        if (go == this.gameObject)
            return;

        _isChoice = false;
        SetColor(GetImage((int)Images.ChoiceFrame), 100f/255f);
    }

    private void OnClickAbilityCard()
    {
        Debug.Log("OnClickAbilityCard");

        _isChoice = true;
        SetColor(GetImage((int)Images.ChoiceFrame), 1f);

        Managers.UI.FindPopup<UI_DrawAbilityPopup>()?._onClickAbilityCard.Invoke(this.gameObject);
    }
    
    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }
}
