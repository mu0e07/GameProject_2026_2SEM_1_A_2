
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;
    [SerializeField] private TextMeshProUGUI selectedItemText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int healAmount = 30;

    [Header("인벤토리 데이터")]
    private List<InventoryItem> items = new List<InventoryItem>();

    [Header("플레이어 연결")]
    private PlayerController playerController;
    private PlayerPickup playerPickup;
    private ThirdPersonCamera thirdPersonCamera;

    private bool isOpen;

   [SerializeField] private int selectedIndex = -1;
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

        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            selectedItemText.text = "선택 : " + items[selectedIndex].itemName;
            descriptionText.text = items[selectedIndex].description;
            useButton.interactable = CanUseItem(items[selectedIndex].itemName);
            dropButton.interactable = true;
        }
        else
        {
            selectedItemText.text = "아이템을 선택하세요";
            descriptionText.text = "슬롯을 클릭하면 설명이 표시 됩니다.";

            useButton.interactable = false;
            dropButton.interactable = false;
        }
    }

    // 아이템 추가

    public bool AddItem(string itemName, int amount, string description)
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
        newItem.description = description;

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

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;

        UpdateUI();
    }

    public void SelectSlot0() { SelectSlot(0); }
    public void SelectSlot1() { SelectSlot(1); }
    public void SelectSlot2() { SelectSlot(2); }
    public void SelectSlot3() { SelectSlot(3); }

    private bool CanUseItem(string itemName)
    {
        return itemName == "물약" || itemName == "회복약" || itemName == "구급키트";
    }

    public bool RemoveItem(string itemName, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName != itemName) continue;  //아이템 이름이 같은지
            if (items[i].amount < amount) return false;   //충분한 수량이 있는지

            items[i].amount -= amount;                    //수량 감소

            if (items[i].amount <= 0)                     //0개면 리스트에서 제거
            {
                items.RemoveAt(i);
                selectedIndex = -1;
            }

            UpdateUI();
            return true;
        }

        return false;
    }
    public void UseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        InventoryItem item = items[selectedIndex];

        if (CanUseItem(item.itemName) == false)
        {
            selectedItemText.text = item.itemName + "은(는) 사용할 수 없습니다.";
            return;
        }

        playerHealth.Heal(healAmount);
        RemoveItem(item.itemName, 1);
    }

    public void DropSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        string itemName = items[selectedIndex].itemName;
        RemoveItem(itemName, 1);
    }
    public int GetItemAmount(string itemName) 
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return item.amount;
            }
        }

        return 0;
    }
}
