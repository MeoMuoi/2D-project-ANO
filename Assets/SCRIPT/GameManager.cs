using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    [Header("Lives Settings")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;

    private UIHealthBar uiHealthBar;
    private UILives uiLives;
    private HeroKnight player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        uiHealthBar = FindAnyObjectByType<UIHealthBar>();
        uiLives = FindAnyObjectByType<UILives>();
        player = FindAnyObjectByType<HeroKnight>();

        currentHealth = maxHealth;
        currentLives = maxLives;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth);
        if (uiLives != null) uiLives.SetLives(currentLives);
    }

    // --- Health ---
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth);

        player?.TriggerHurt();

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth);
    }

    // --- Death & Respawn ---
    private void Die()
    {
        player?.TriggerDeath();

        currentLives--;
        if (uiLives != null) uiLives.SetLives(currentLives);

        if (currentLives > 0) Respawn();
        else Debug.Log("Game Over!");
    }

    private void Respawn()
    {
        if (respawnPoint != null && player != null)
        {
            player.transform.position = respawnPoint.position;
        }

        currentHealth = maxHealth;
        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth);
    }
    // --- Lives ---
    public void AddLife(int amount)
    {
        currentLives = Mathf.Min(currentLives + amount, maxLives);
        if (uiLives != null) uiLives.SetLives(currentLives);
    }

}
