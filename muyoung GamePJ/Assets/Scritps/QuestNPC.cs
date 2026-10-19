
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestNPC : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private Quest quest;

    private bool playerNear;

    private void Update()
    {
        if (!playerNear) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            if (questManager != null)
            {
                questManager.TalkToNPC(quest);
            }
            else
            {
                Debug.LogWarning("QuestManager가 연결되지 않았습니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            Debug.Log("E 키를 눌러 대화하세요.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}
