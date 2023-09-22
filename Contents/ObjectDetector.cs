using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner    towerSpawner;

    private Camera          mainCamera;
    private Ray             ray;
    private RaycastHit      hit;

    private int _mercenaryMask = (1 << (int)Define.LayerType.Mercenary);
    private int _tileMask = (1 << (int)Define.LayerType.Tile);

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        OnTowerChoice();    // 타워 선택
        TileTowerSpawn();   // 타워 소환
    }

    // 타워 들기
    float currentMouseTime = 0f;
    float maxMouseTime = 0.3f;
    private void OnTowerChoice()
    {
        // 마우스 입력 확인
        if (Input.GetMouseButton(0) == false)
        {
            currentMouseTime = 0f;
            return;
        }

        // 드래그 중인가?
        if (Managers.Game.isDrag == true)
            return;

        // 마우스 위치에 용병이 존재한가?
        if (RayMousePointCheck(_mercenaryMask) == false)
            return;

        // 들기 준비
        currentMouseTime += Time.deltaTime;
        if (currentMouseTime >= maxMouseTime)
        {
            // 들기 시작
            MercenaryController mercenary = hit.transform.GetComponent<MercenaryController>();
            mercenary.currentTile.IsBuildTower = false;

            MercenaryStat stat = mercenary.GetMercenaryStat();

            // 들기 정보 입력
            UI_DragSlot.instance.mercenartObj = mercenary.gameObject;
            UI_DragSlot.instance.mercenaryStat = stat;

            Managers.Game.isDrag = true;
        }
    }

    // 타워 생성
    private void TileTowerSpawn()
    {
        // 마우스 입력 확인
        if (Input.GetMouseButtonUp(0) == false)
            return;

        // 드래그 슬롯 정보 확인
        if (UI_DragSlot.instance.GetMercenary().IsNull() == true)
        {
            Debug.Log("Drag Slot Info Failed");
            return;
        }

        // 마우스 위치 타일 확인
        if (RayMousePointCheck(_tileMask) == false)
            return;

        // 타워 생성
        if (towerSpawner.SpawnTower(hit.transform) == true)
        {
            // 용병을 옮기는 것이라면 기존 용병 삭제
            if (UI_DragSlot.instance.mercenartObj.IsFakeNull() == false)
            {
                Managers.Resource.Destroy(UI_DragSlot.instance.mercenartObj);
                UI_DragSlot.instance.mercenartObj = null;
                Debug.Log("Temp Mercenary Destroy");
            }
        }
    }

    // 마우스 포인트 Ray 확인
    private bool RayMousePointCheck(int mask)
    {
        // 카메라에서 마우스 위치를 바라보는 ray 생성
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // ray에 부딪친 Layer가 _maks라면 통과
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            Debug.Log(hit.transform.name + " : Hit True!!");
            return true;
        }

        return false;
    }
}
