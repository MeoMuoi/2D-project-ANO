using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int maxHealth = 5;   
    public int maxLives = 3;    

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    private int currentHealth;
    private int currentLives;

    private UIHealthBar uiHealthBar;
    private UILives uiLives;
    private MainCharacter player;

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
        // Unity 2022 trở lên: dùng FindFirstObjectByType thay vì FindObjectOfType (bị warning)
        uiHealthBar = FindFirstObjectByType<UIHealthBar>();
        uiLives = FindFirstObjectByType<UILives>();
        player = FindFirstObjectByType<MainCharacter>();

        currentHealth = maxHealth;
        currentLives = maxLives;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.J)) Heal(1);
        if (Input.GetKeyDown(KeyCode.L)) AddLife(1);
    }

    // ========================= HEALTH =========================
    public void TakeDamage(int damage)
    {
        if (player == null) return;

        // ❌ KHÔNG được gọi nếu Player đang lăn hoặc có I-frames
        if (player.IsInvincible)
            return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            PlayerDied();
        }
        else
        {
            player.TriggerHurt();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);
    }

    // ========================= LIVES =========================
    private void PlayerDied()
    {
        currentLives--;
        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);

        if (player != null)
        {
            player.SetControl(false);
            StartCoroutine(HandleDeathAndRespawnQuick());
        }

        if (currentLives <= 0)
        {
            Debug.Log("GAME OVER");
        }
    }

    // ========================= DEATH + RESPAWN =========================
    private IEnumerator HandleDeathAndRespawnQuick()
    {
        if (player == null) yield break;

        Animator animator = player.GetComponent<Animator>();
        if (animator == null) yield break;

        animator.ResetTrigger("Hurt");
        animator.CrossFade("Death", 0f);

        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            yield return null;

        float deathDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathDuration);

        // Reset máu
        currentHealth = maxHealth;
        if (uiHealthBar != null) uiHealthBar.SetHealth(currentHealth, maxHealth);

        // Respawn
        if (respawnPoint != null)
            player.transform.position = respawnPoint.position;
        else
            Debug.LogWarning("RespawnPoint is NOT assigned!");

        animator.CrossFade("Idle", 0f);
        player.SetControl(true);

        Debug.Log("Respawn thành công! Lives còn: " + currentLives);
    }

    public void AddLife(int amount)
    {
        currentLives += amount;
        if (currentLives > maxLives) currentLives = maxLives;

        if (uiLives != null) uiLives.SetLives(currentLives, maxLives);
    }
}
