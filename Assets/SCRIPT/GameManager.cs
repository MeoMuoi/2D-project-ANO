using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int maxHealth = 5;   // 5 vạch máu
    public int maxLives = 3;    // 3 mạng

    [Header("Respawn Settings")]
    [Tooltip("Kéo thả một Empty Object làm vị trí respawn cho Player")]
    public Transform respawnPoint;
    public float respawnDelay = 2f; // thời gian chờ sau khi chết

    private int currentHealth;
    private int currentLives;

    private UIHealthBar uiHealthBar;
    private UILives uiLives;
    private HeroKnight player;

    void Awake()
    {
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

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }

    void Update()
    {
        // Test phím (chỉ để debug)
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(1);  // mất 1 máu
        if (Input.GetKeyDown(KeyCode.J)) Heal(1);        // hồi 1 máu
        if (Input.GetKeyDown(KeyCode.L)) AddLife(1);     // +1 mạng
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
        else
        {
            if (player != null) player.TriggerHurt();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
    }

    // =========================
    // Lives & Respawn
    // =========================
    private void PlayerDied()
    {
        currentLives--;
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);

        if (player != null)
        {
            player.TriggerDeath();
            player.SetControl(false); // disable điều khiển khi chết
        }

        if (currentLives > 0)
        {
            StartCoroutine(RespawnCoroutine());
        }
        else
        {
            Debug.Log("Game Over!");
            // TODO: load màn hình Game Over scene nếu muốn
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        currentHealth = maxHealth;
        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);

        if (respawnPoint != null && player != null)
        {
            player.transform.position = respawnPoint.position;
        }

        if (player != null) player.SetControl(true); // bật lại điều khiển

        Debug.Log("Respawn player, lives left: " + currentLives);
    }

    public void AddLife(int amount)
    {
        currentLives += amount;
        if (currentLives > maxLives) currentLives = maxLives;

        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }
}
