using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Nếu bạn dùng TextMeshPro cho UI

public class MainMenuManager : MonoBehaviour
{
    // Đây là đối tượng giữ tên game "HÀNH LANG CHẾT CHÓC"
    [Header("Game Title")]
    public TextMeshProUGUI gameTitleText; 
    
    [Header("UI Buttons")]
    public Button playButton;       // CHƠI
    public Button settingsButton;   // Cài Đặt
    public Button creditsButton;    // Danh đề (Credit)
    public Button quitButton;       // Tùy chọn: Thoát

    [Header("Scene Settings")]
    [Tooltip("Tên Scene sẽ được tải khi nhấn CHƠI (Ví dụ: Level_1)")]
    public string firstLevelName = "Level_1"; 
    
    [Tooltip("Tên Scene cho phần Credit")]
    public string creditsSceneName = "Credits";

    void Start()
    {
        // Khởi tạo tên game
        if (gameTitleText != null)
        {
            gameTitleText.text = "HÀNH LANG CHẾT CHÓC";
        }
        
        // Gán các hàm vào sự kiện OnClick của từng nút
        if (playButton != null) playButton.onClick.AddListener(StartGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (creditsButton != null) creditsButton.onClick.AddListener(OpenCredits);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        
        // Luôn hiển thị con trỏ chuột trong Menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Debug.Log("Starting Game: HÀNH LANG CHẾT CHÓC...");
        // Tải Scene màn chơi đầu tiên
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenSettings()
    {
        Debug.Log("Opening Settings Menu...");
        // TODO: Viết logic để bật Panel Settings hoặc Load Scene Settings
    }

    public void OpenCredits()
    {
        Debug.Log("Loading Credits...");
        if (!string.IsNullOrEmpty(creditsSceneName))
        {
            SceneManager.LoadScene(creditsSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Application...");
        
        // Thoát ứng dụng
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}