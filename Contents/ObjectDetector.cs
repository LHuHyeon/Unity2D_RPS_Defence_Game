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

    private int _tileMask       = (1 << (int)Define.LayerType.Tile);

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
    private void OnTowerChoice()
    {
        // 마우스 입력 확인
        if (Input.GetMouseButton(0) == false)
            return;

        // 드래그 중인가?
        if (Managers.Game.isDrag == true)
            return;

        // 마우스 위치에 타일이 존재한가?
        if (RayMousePointCheck((_tileMask)) == false)
            return;

        // 타일에 용병이 존재한가?
        if (hit.transform.GetComponent<Tile>().mercenaryObj.IsFakeNull() == true)
            return;

        // 들기 시작
        Managers.Game.isDrag = true;
                
        // 타일 가져오기
        Tile tile = hit.transform.GetComponent<Tile>();

        // 들기 정보 입력
        UI_DragSlot.instance.tile = tile;
        UI_DragSlot.instance.icon.sprite = tile.GetMercenary().GetStat().Icon;
    }

    // 타워 생성
    private void TileTowerSpawn()
    {
        // 마우스 입력 확인
        if (Input.GetMouseButtonUp(0) == false)
            return;

        UI_DragSlot dragSlot = UI_DragSlot.instance;

        // 드래그 슬롯 정보 확인
        if (dragSlot.GetMercenary().IsNull() == true)
        {
            Debug.Log("DragSlot Info Failed");
            return;
        }

        // 마우스 위치 타일 확인
        if (RayMousePointCheck(_tileMask) == false)
        {
            dragSlot.DragInfoClear();
            return;
        }

        // 타일에서 온거면 교체
        if (dragSlot.tile.IsFakeNull() == false)
        {
            Tile tile = hit.transform.GetComponent<Tile>();
            tile.TileChange(dragSlot.tile);
            dragSlot.DragInfoClear();
            return;
        }

        // 타워 생성
        if (towerSpawner.SpawnTower(dragSlot.GetMercenary(), hit.transform) == false)
            return;
        
        // 들고 있는 용병이 슬롯에서 온거면 개수 차감
        if ((dragSlot.itemSlot is UI_MercenaryItem) == true)
        {
            UI_MercenaryItem mercenaryItem = dragSlot.itemSlot as UI_MercenaryItem;
            mercenaryItem.SetCount(-1);
            dragSlot.DragInfoClear();
        }
    }

    // 마우스 포인트 Ray 확인
    private bool RayMousePointCheck(int mask)
    {
        // 카메라에서 마우스 위치를 바라보는 ray 생성
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // ray에 부딪친 Layer가 _maks라면 통과
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            return true;

        return false;
    }
}
