using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_AbilityListPopup : UI_Popup
{
    enum GameObjects
    {
        ExitBackground,
        Background,
        AbilityListContent,
    }

    private Dictionary<Define.AbilityType, UI_AbilitySlot> _abilitySlots = new Dictionary<Define.AbilityType, UI_AbilitySlot>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));

        GetObject((int)GameObjects.ExitBackground).BindEvent(OnClickExitButton);
        
        foreach(Transform child in GetObject((int)GameObjects.AbilityListContent).transform)
            Managers.Resource.Destroy(child.gameObject);

        RefreshUI();

        return true;
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        // 현재 획득한 능력 채우기
        PopulateAbilitySlot();

        StartCoroutine(CallPopup());
    }

    private void PopulateAbilitySlot()
    {
        List<AbilityData> abilities = Managers.Game.Abilities;

        // 현재 등록된 능력값들 비우기
        foreach(var slot in _abilitySlots)
            slot.Value.Clear();

        // 획득한 능력을 List로 생성 및 값 추가
        for(int i=0; i<abilities.Count; i++)
        {
            AbilityData ability = abilities[i];

            // 능력Type의 슬롯이 없으면 생성
            UI_AbilitySlot abilitySlot;
            if (_abilitySlots.TryGetValue(ability.abilityType, out abilitySlot) == false)
            {
                abilitySlot = Managers.UI.MakeSubItem<UI_AbilitySlot>(GetObject((int)GameObjects.AbilityListContent).transform);
                abilitySlot.SetInfo(ability.AbilityClone());

                _abilitySlots.Add(ability.abilityType, abilitySlot);
            }

            // 문자열 값 적용
            abilitySlot.RefreshDescripition(ability.currentValue);
        }

        // UI 새로고침
        foreach(var slot in _abilitySlots)
            slot.Value.RefreshUI();
    }

    private void OnClickExitButton(PointerEventData eventData)
    {
        Debug.Log("OnClickExitButton");

        Clear();
    }

    private IEnumerator CallPopup()
    {
        // CallPopup : Background 투명도가 활성화되면 그 다음 Content 객체 활성화
        
        GetObject((int)GameObjects.AbilityListContent).SetActive(false);

        yield return null;

        GetObject((int)GameObjects.AbilityListContent).SetActive(true);

    }

    public void Clear()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
