
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;

    [Header("인벤토리 데이터")]
    private List<InventoryItem> items = new List<InventoryItem>();

    [Header("플레이어 연결")]
    private PlayerController playerController;
    private PlayerPickup playerPickup;
    private ThirdPersonCamera thirdPersonCamera;

    private bool isOpen;

    // 초기 설정
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }

    // 게임 시작 시 UI 갱신
    private void Start()
    {
        UpdateUI();
    }

    // 인벤토리 열기 / 닫기
    public void OnInventory(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleInventory();
        }
    }

    // 인벤토리 상태 변경

    private void ToggleInventory()
    {
        // 인벤토리를 열 때 일반 상태가 아니면 열리지 않도록 설정
        if (isOpen == false && playerController.IsNormalState() == false)
        {
            return;
        }

        isOpen = !isOpen;

        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            // 인벤토리 열기
            playerController.ChangeState(PlayerState.inventory);
            playerPickup.enabled = false;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            UpdateUI();
        }
        else
        {
            // 인벤토리 닫기
            playerController.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    // 인벤토리 UI 업데이트
    private void UpdateUI()
    {
        for (int i = 0; i < slotText.Length; i++)
        {
            if (i < items.Count)
            {
                slotText[i].text =
                    items[i].itemName + " x " + items[i].amount;
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        }
    }

    // 아이템 추가

    public bool AddItem(string itemName, int amount)
    {
        // 같은 이름의 아이템이 있는지 확인
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }

        // 인벤토리가 가득 찼는지 확인
        if (items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        // 새로운 아이템 생성
        InventoryItem newItem = new InventoryItem();

        newItem.itemName = itemName;
        newItem.amount = amount;

        // 인벤토리에 추가
        items.Add(newItem);

        UpdateUI();

        return true;
    }


    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        // 키보드가 연결되지 않았다면 종료
        if (keyboard == null) return;

        // I 키를 누르면 인벤토리 열기 / 닫기
        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    // 인벤토리 열림 상태 확인
    public bool IsOpen()
    {
        return isOpen;
    }
}
