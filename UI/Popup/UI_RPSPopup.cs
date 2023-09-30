using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RPSPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        RPSBg,
        RPSGrid,
        MercenaryGrid,
    }

    enum Buttons
    {
        HelperButton,
        ResetButton,
        CheckButton,
    }

    private int maxCardCount = 5;

    private List<UI_RPSCard> _cardList = new List<UI_RPSCard>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.HelperButton).onClick.AddListener(OnClickHelperButton);
        GetButton((int)Buttons.ResetButton).onClick.AddListener(OnClickResetButton);
        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);

        InitClear();
        PopulateRPSCard();

        RPSCardSetting();

        return true;
    }

    public void RPSCardSetting()
    {
        for(int i=0; i<_cardList.Count; i++)
            _cardList[i].SetInfo();
    }

    private void InitClear()
    {
        foreach(Transform child in GetObject((int)GameObjects.MercenaryGrid).transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    private void PopulateRPSCard()
    {
        Transform rpsGrid = GetObject((int)GameObjects.RPSGrid).transform;

        foreach(Transform child in rpsGrid)
            Managers.Resource.Destroy(child.gameObject);
        
        for(int i=0; i<maxCardCount; i++)
        {
            UI_RPSCard rpsCard = Managers.UI.MakeSubItem<UI_RPSCard>(rpsGrid);
            rpsCard.SetInfo();

            _cardList.Add(rpsCard);
        }
    }

    private void OnClickHelperButton()
    {
        Debug.Log("OnClickHelperButton");
    }

    private void OnClickResetButton()
    {
        Debug.Log("OnClickResetButton");

        for(int i=0; i<_cardList.Count; i++)
            _cardList[i].OnRandomCard();
    }

    private void OnClickCheckButton()
    {
        Debug.Log("OnClickCheckButton");

		Managers.Game.WaveSystem.WaveStart();

        Managers.Resource.Destroy(this.gameObject);
    }
}
