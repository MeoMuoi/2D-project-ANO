using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDialogue : MonoBehaviour
{
    public static UIDialogue Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;   // Panel chứa UI thoại
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject arrowIndicator;

    private string[] lines;
    private int currentLine = 0;
    private System.Action onDialogueEnd;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Đảm bảo panel ẩn khi bắt đầu
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (arrowIndicator != null)
            arrowIndicator.SetActive(false);
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void ShowDialogue(string[] dialogueLines, Sprite portrait, System.Action onEnd = null)
    {
        if (dialoguePanel == null || dialogueText == null) return;

        lines = dialogueLines;
        currentLine = 0;
        onDialogueEnd = onEnd;

        // Set portrait
        if (portraitImage != null && portrait != null)
            portraitImage.sprite = portrait;

        // Hiện panel
        dialoguePanel.SetActive(true);

        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
            if (arrowIndicator != null)
                arrowIndicator.SetActive(true);
        }
        else
        {
            EndDialogue();
        }
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (arrowIndicator != null)
            arrowIndicator.SetActive(false);

        onDialogueEnd?.Invoke();
    }
}
