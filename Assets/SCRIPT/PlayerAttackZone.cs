using UnityEngine;

public class PlayerAttackZone : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 1;

    private bool hasHit = false; // Ngăn chặn tấn công nhiều lần trong 1 lần va chạm

    void OnEnable()
    {
        // Reset khi vùng tấn công được bật (ví dụ: từ Animation Event)
        hasHit = false; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 1. Cố gắng lấy Component Enemy
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            // 2. Gây sát thương cho Enemy
            enemy.TakeDamage(attackDamage);

            // 3. Đánh dấu đã trúng (nếu chỉ muốn đánh 1 mục tiêu mỗi lần vung kiếm)
            hasHit = true; 
        }
    }
    
    // Lưu ý: Cần đảm bảo GameObject chứa script này được bật/tắt đúng lúc 
    // thông qua Animation Event trong script MainCharacter.cs
}