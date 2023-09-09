using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner    towerSpawner;

    [SerializeField]
    private GameObject      selectHelper;   // 타일 선택 도우미

    private Camera          mainCamera;
    private Ray             ray;
    private RaycastHit      hit;

    private int _mask = (1 << (int)Define.LayerType.Mercenary);

    void Start()
    {
        mainCamera = Camera.main;

        selectHelper = Instantiate(selectHelper);
    }

    void Update()
    {
        TileSelect();
    }

    private void TileSelect()
    {
        // 카메라에서 마우스 위치를 바라보는 ray 생성
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // ray에 부딪친 Layer가 _maks라면 통과
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _mask))
        {
            OnSelectHelper(true, hit.transform);

            if (Input.GetMouseButtonDown(0))
            {
                // 타워 생성
                towerSpawner.SpawnTower(hit.transform);
            }
        }
        else
            OnSelectHelper(false);
    }

    // 타일 선택을 눈으로 쉽게 보여주는 도우미 함수
    private void OnSelectHelper(bool isActivation, Transform pos = null)
    {
        selectHelper.SetActive(isActivation);

        if (isActivation == true)
            selectHelper.transform.position = pos.position;
    }
}
