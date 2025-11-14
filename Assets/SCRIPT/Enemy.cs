using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int attackDamage = 1; // Mức sát thương gây ra cho Player
    public int maxHealth = 3;
    private int currentHealth;
    private Transform player;

    private void Start()
    {
        currentHealth = maxHealth;
        // LƯU Ý: FindGameObjectWithTag là chậm. Nên dùng GameManager lưu trữ tham chiếu.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); 
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player != null)
        {
            // Di chuyển về phía player
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); // Hủy Enemy (Đúng)
    }

    // ✅ THÊM: Xử lý va chạm với Player để gây sát thương
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với Player
        MainCharacter playerScript = collision.gameObject.GetComponent<MainCharacter>();

        if (playerScript != null)
        {
            // Gọi hàm TakeDamage của GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TakeDamage(attackDamage);
            }
            // Không được Destroy(gameObject) của Player ở đây!
        }
    }
}