using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public string[] dialogueLines;
    public Sprite portrait;   // Ảnh chân dung NPC

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (UIDialogue.Instance != null)
            {
                UIDialogue.Instance.ShowDialogue(dialogueLines, portrait);
            }
            else
            {
                Debug.LogError("UIDialogue.Instance chưa được set trong scene!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
