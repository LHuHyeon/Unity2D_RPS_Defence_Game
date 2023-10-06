using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RPSPopup : UI_Popup
{
    class RPSCard
    {
        public Define.RPSCard cardType;     // 카드 타입
        public Define.GradeType cardGrade;  // 카드 등급
        public bool isCard;                 // 카드가 뽑혔는지

        public void AddCard()   // 카드 추가
        {
            cardGrade++;
            isCard = true;
        }

        public void Clear(Define.RPSCard card)
        {
            cardType = card;
            cardGrade = Define.GradeType.Basic;
            isCard = false;
        }
    }

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

    private int     maxCardCount    = 5;        // 카드 최대 개수
    private bool    isCheckButton   = false;
    private bool    isResetButton   = true;

    private List<UI_RPSCard> _cardList = new List<UI_RPSCard>();
    private List<MercenaryStat> _rewardMercenary = new List<MercenaryStat>();
    private Dictionary<Define.RPSCard, RPSCard> _rpsCardDict;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.HelperButton).onClick.AddListener(OnClickHelperButton);
        GetButton((int)Buttons.ResetButton).onClick.AddListener(OnClickResetButton);
        GetButton((int)Buttons.CheckButton).onClick.AddListener(OnClickCheckButton);

        _rpsCardDict = new Dictionary<Define.RPSCard, RPSCard>()
        {
            {Define.RPSCard.Scissors, new RPSCard()},
            {Define.RPSCard.Rock, new RPSCard()},
            {Define.RPSCard.Paper, new RPSCard()},
        };

        PopulateRPSCard();

        RefreshUI();

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        // RPS 카드 세팅
        for(int i=0; i<_cardList.Count; i++)
            _cardList[i].SetInfo();

        // 용병 슬롯 삭제
        foreach(Transform child in GetObject((int)GameObjects.MercenaryGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        SetColor(GetButton((int)Buttons.ResetButton).image, 1f);
        GetButton((int)Buttons.CheckButton).image.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Btn_DarkGray");

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

    private void SetColor(Image icon, float alpha)
    {
        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    private void OnClickHelperButton()
    {
        Debug.Log("OnClickHelperButton");
    }

    private void OnClickResetButton()
    {
        Debug.Log("OnClickResetButton");
        
        if (isResetButton == false)
            return;

        isResetButton = false;

        // 카드 리셋
        for(int i=0; i<_cardList.Count; i++)
            _cardList[i].OnRandomCard();

        // 투명화
        SetColor(GetButton((int)Buttons.ResetButton).image, 0.3f);
    }

    private void OnClickCheckButton()
    {
        Debug.Log("OnClickCheckButton");

        if (isCheckButton == false)
        {
            // 첫 확인 누를 시 리셋 버튼 비활성화
            isCheckButton = true;
            isResetButton = false;
            GetButton((int)Buttons.CheckButton).image.sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Btn_BlueGray");
            SetColor(GetButton((int)Buttons.ResetButton).image, 0.3f);

            // 보상 확인
            GetCardReward();
        }
        else
        {
            // 보상 용병 등록
            for(int i=0; i<_rewardMercenary.Count; i++)
                Managers.Game.GameScene.MercenaryRegister(_rewardMercenary[i]);
            
            // 웨이브 시작
            Managers.Game.WaveSystem.WaveStart();
            Clear();
        }
    }

    // 카드 보상 확인
    private void GetCardReward()
    {
        // 초기화
        _rewardMercenary = new List<MercenaryStat>();
        foreach(var rpsCard in _rpsCardDict)
            rpsCard.Value.Clear(rpsCard.Key);

        // 현재 카드 정보 가져오기
        foreach(UI_RPSCard rpsCard in _cardList)
            _rpsCardDict[rpsCard.GetCard()].AddCard();

        // 계산 오류 없도록 임시 변수 생성
        Define.GradeType tempScissors   = _rpsCardDict[Define.RPSCard.Scissors].cardGrade;
        Define.GradeType tempRock       = _rpsCardDict[Define.RPSCard.Rock].cardGrade;
        Define.GradeType tempPaper      = _rpsCardDict[Define.RPSCard.Paper].cardGrade;

        // 패배한 카드 차감 시켜주기 ( 가위3 - 주먹1 = 가위2 )
        _rpsCardDict[Define.RPSCard.Scissors].cardGrade -= tempRock;
        _rpsCardDict[Define.RPSCard.Rock].cardGrade -= tempPaper;
        _rpsCardDict[Define.RPSCard.Paper].cardGrade -= tempScissors;

        // 보상 용병 획득
        foreach(var rpsCard in _rpsCardDict)
            RewardMercenary(rpsCard.Value);
    }

    // 용병 보상 
    private void RewardMercenary(RPSCard card)
    {
        // 카드가 뽑히지 않았다면
        if (card.isCard == false)
            return;

        // 카드 개수가 0 미만이면 Basic 반환
        if ((int)card.cardGrade < 0)
            card.cardGrade = Define.GradeType.Basic;

        Debug.Log("Card : " + card.cardType.ToString() + ", Grade : " + card.cardGrade.ToString());

        // 카드 정보로 용병 가져오기
        List<MercenaryStat> mercenarys = Managers.Data.GetMercenarys(card.cardGrade, (Define.JobType)card.cardType);

        // 랜덤 용병 뽑기
        MercenaryStat mercenary = mercenarys[Random.Range(0, mercenarys.Count)];
        _rewardMercenary.Add(mercenary);

        // 용병 View Slot 생성
        Transform parent = GetObject((int)GameObjects.MercenaryGrid).transform;
        UI_MercenaryViewSlot viewSlot = Managers.UI.MakeSubItem<UI_MercenaryViewSlot>(parent);
        viewSlot.SetInfo(mercenary);
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
        isCheckButton = false;
        isResetButton = true;
        Managers.Resource.Destroy(this.gameObject);
    }
}
