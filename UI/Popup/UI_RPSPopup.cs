using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int maxCardCount = 5;   // 카드 최대 개수

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

        PopulateRPSCard();
        
        // 카드 세팅
        for(int i=0; i<_cardList.Count; i++)
            _cardList[i].SetInfo();

        RefreshUI();

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        // 용병 슬롯 삭제
        foreach(Transform child in GetObject((int)GameObjects.MercenaryGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        StartCoroutine(CallPopup());
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

        Clear();
    }

    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }
 
    // 팝업 호출 시 등장!
    private float maxAlpha = 0.5f;  // 투명도 최대치
    private float lerpTime = 0.7f;  // Lerp 시간
    private IEnumerator CallPopup()
    {
        // 팝업 비활성화
        GetObject((int)GameObjects.RPSBg).SetActive(false);

        // 배경 가져오기
        Transform background = GetObject((int)GameObjects.Background).transform;
        Image icon = background.GetComponent<Image>();

        SetColor(icon, 0);

        // 배경 어둡게
        float currentAlpha = 0f;
        while (currentAlpha < maxAlpha)
        {
            yield return null;

            currentAlpha += 0.07f;
            SetColor(icon, currentAlpha);
        }
        
        // 팝업 활성화
        GetObject((int)GameObjects.RPSBg).SetActive(true);

        // 올라오며 팝업 등장
        float currentTime = 0;
        while (currentTime < lerpTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / lerpTime;
            
            t = Mathf.Sin(t * Mathf.PI * 0.5f);     // SmoothStep 참고 

            background.localPosition = Vector3.Lerp(Vector3.up * -100f, Vector3.zero, t);

            yield return null;
        }
    }

    public void Clear()
    {
        Managers.Resource.Destroy(this.gameObject);
    }
}
