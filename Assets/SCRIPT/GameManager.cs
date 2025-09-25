using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int maxHealth = 5;   // số mức máu (match số sprite trong UIHealthBar)
    public int maxLives = 3;    // số mạng (match số sprite trong UILives)

    private int currentHealth;
    private int currentLives;

    private UIHealthBar uiHealthBar;
    private UILives uiLives;
    private HeroKnight player;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uiHealthBar = FindObjectOfType<UIHealthBar>();
        uiLives = FindObjectOfType<UILives>();
        player = FindObjectOfType<HeroKnight>();

        currentHealth = maxHealth;
        currentLives = maxLives;

        // Khởi tạo UI
        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }

    // =========================
    // Health
    // =========================
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            PlayerDied();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
    }

    // =========================
    // Lives
    // =========================
    private void PlayerDied()
    {
        currentLives--;
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);

        if (currentLives > 0)
        {
            // Respawn
            currentHealth = maxHealth;
            if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
            if (player != null) player.transform.position = Vector3.zero; // hoặc respawnPoint
        }
        else
        {
            Debug.Log("Game Over!");
            // TODO: load màn hình Game Over
        }
    }

    public void AddLife(int amount)
    {
        currentLives += amount;
        if (currentLives > maxLives) currentLives = maxLives;

        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }
}
