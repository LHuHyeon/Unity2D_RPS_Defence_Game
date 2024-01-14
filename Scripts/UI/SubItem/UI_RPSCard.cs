using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_RPSCard.cs
 * Desc :   "UI_RPSPopup"의 하위 항목으로 사용
 *          가위바위보 카드의 기능을 수행한다.
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : GetCard()             - 카드값 반환
 &  : SetInfo()             - 정보 설정
 &  : RefreshUI()           - UI 새로고침
 &  : RefreshRPSIcon()      - 가위바위보 Icon 새로고침
 &  : RefreshResetColor()   - 리셋 버튼 컬러 새로고침(활성화/비활성화)
 &  : OnRandomCard()        - 카드 랜덤 설정
 &
 &  [Private]
 &  : CardRotation()        - 카드 랜덤 설정 시 효과 (카드 돌리기)
 &  : SetColor()            - 컬러 설정(투명도)
 &  : OnClickResetButton()  - 리셋 버튼
 *
 */
 
public class UI_RPSCard : UI_Base
{
    enum GameObjects
    {
        CardBg,
    }

    enum Buttons
    {
        RPSResetButton,
    }

    enum Images
    {
        RPSIcon,
        RPSResetButtonIcon,
    }

    public Define.RPSCard rpsType = Define.RPSCard.Unknown;

    private bool _isReset = true;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.RPSResetButton).onClick.AddListener(OnClickResetButton);

        RefreshUI();

        return true;
    }

    public Define.RPSCard GetCard()
    {
        // 리셋 비활성화
        _isReset = false;
        RefreshResetColor();

        return rpsType;
    }

    public void SetInfo()
    {
        _isReset = true;
        rpsType = (Define.RPSCard)Random.Range(1, (int)Define.RPSCard.Max);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        // 카드 회전 초기화
        GetObject((int)GameObjects.CardBg).transform.localRotation = Quaternion.identity;

        RefreshRPSIcon();
        RefreshResetColor();
    }

    // 가위바위보 이미지
    public void RefreshRPSIcon()
    {
        if (rpsType == Define.RPSCard.Unknown)
            GetImage((int)Images.RPSIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Question");
        else
            GetImage((int)Images.RPSIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/"+rpsType.ToString());
    }

    // ResetButton 투명도
    public void RefreshResetColor()
    {
        if (_isReset == true)
        {
            SetColor(GetButton((int)Buttons.RPSResetButton).image, 1);
            SetColor(GetImage((int)Images.RPSResetButtonIcon), 1);
        }
        else
        {
            SetColor(GetButton((int)Buttons.RPSResetButton).image, 0.25f);
            SetColor(GetImage((int)Images.RPSResetButtonIcon), 0.25f);
        }
    }

    // 카드 랜덤 설정
    private Coroutine co;
    public void OnRandomCard(bool isReset = false)
    {
        // 리셋을 이미 사용했다면? (광고 보면 리셋 한번 더 가능하게!)
        if (_isReset == false)
        {
            _isReset = isReset;
            RefreshResetColor();
        }

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(CardRotation());
    }

    // 카드 돌리기
    private float cardRotationSpeed = 130f;
    private IEnumerator CardRotation()
    {
        Transform bg = GetObject((int)GameObjects.CardBg).transform;

        float rotationY = -180f;
        bg.localRotation = Quaternion.Euler(0, rotationY, 0);

        GetImage((int)Images.RPSIcon).sprite = Managers.Resource.Load<Sprite>("UI/Sprite/Question");

        // 절반 회전
        while(rotationY < -90f)
        {
            yield return null;

            rotationY += cardRotationSpeed * Time.deltaTime;
            bg.localRotation = Quaternion.Euler(0, rotationY, 0);
        }

        // 카드 랜덤 세팅
        rpsType = (Define.RPSCard)Random.Range(1, (int)Define.RPSCard.Max);
        RefreshRPSIcon();

        // 절반 회전
        while(rotationY < 0f)
        {
            yield return null;

            rotationY += cardRotationSpeed * Time.deltaTime;
            bg.localRotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    // 투명도 설정 (0.1 ~ 1)
    private void SetColor(Image icon, float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }

    private void OnClickResetButton()
    {
        if (_isReset == false)
            return;

        _isReset = false;
        RefreshResetColor();

        OnRandomCard();
    }
}
