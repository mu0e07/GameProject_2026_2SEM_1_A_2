using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionUI interactionUI;

    [Header("아이템 탐색")]
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;

    private PickupItem targetItem;
    private Door targetDoor;
    private NPC targetNPC;

    private bool isPickingUp;

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        interactionUI.HideGuide();

        PickupItem itemToCollect = targetItem;

        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y = 0;

        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        playerController.ChangeState(PlayerState.Pickup);
        animator.CrossFade("PickUp01", 0.1f);
        yield return new WaitForSeconds(pickupMoment);
        if (itemToCollect != null)
        {
            itemToCollect.Collect();
        }
        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        animator.CrossFade("Blend Tree", 0.1f);
        playerController.ChangeState(PlayerState.Normal);

        targetItem = null;
        targetDoor = null;
        targetNPC = null; 
        isPickingUp = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickingUp) return;

        targetItem = FindClosestItem();      
        targetDoor = FindClosestDoor();       
        targetNPC = FindClosestNPC();          

        ShowGuide();

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (targetItem == null && targetDoor == null && targetNPC == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interact();                       
        }
    }

    private PickupItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach(Collider itemCollider in colliders)
        {
            PickupItem item = itemCollider.GetComponentInParent<PickupItem>();

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }
        return closestItem;
    }
    private Door FindClosestDoor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); //구체의 충돌체를 만든다.
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders) //충돌 구체에 들어온 모든것을 검사
        {
            Door door = targetCollider.GetComponentInParent<Door>(); //컴포넌트가 Door를 검사

            if (door == null) continue;

            float distance = Vector3.Distance(transform.position, door.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDoor = door;
            }
        }

        return closestDoor;
    }
    private NPC FindClosestNPC()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); //구체의 충돌체를 만든다.
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders) //충돌 구체에 들어온 모든것을 검사
        {
            NPC npc = targetCollider.GetComponentInParent<NPC>(); //컴포넌트가 NPC를 검사

            if (npc == null) continue;

            float distance = Vector3.Distance(transform.position, npc.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }

        return closestNPC;
    }
    private void ShowGuide()
    {
        if (targetItem != null)
        {
            interactionUI.ShowGuide("[E] " + targetItem.GetItemName() + " 줍기");
        }
        else if (targetDoor != null)
        {
            interactionUI.ShowGuide(targetDoor.GetGuideText());
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowGuide(targetNPC.GetGuideText());
        }
        else
        {
            interactionUI.HideGuide();
        }
    }
    private void Interact()  //인터랙션 함수
    {
        if (targetItem != null)
        {
            StartCoroutine(PickupAnimation());  //기존 픽업
        }
        else if (targetDoor != null)
        {
            targetDoor.Interact();  //문 오픈
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowDialogue(targetNPC.GetNPCName(), targetNPC.GetMessage());  //다이얼로그 오픈
        }
    }
}
