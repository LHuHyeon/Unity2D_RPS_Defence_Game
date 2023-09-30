using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private bool isReset = true;

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

    public void SetInfo()
    {
        isReset = true;
        rpsType = (Define.RPSCard)Random.Range(1, (int)Define.RPSCard.Max);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

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
        if (isReset == true)
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
    public void OnRandomCard()
    {
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
        if (isReset == false)
            return;

        isReset = false;
        RefreshResetColor();

        OnRandomCard();
    }
}
