using UnityEngine;
using System.Collections;

// ✅ CẬP NHẬT: Đổi tên class để khớp với tên file (giả định file là Sensor_MainCharacter.cs)
public class Sensor_MainCharacter : MonoBehaviour {

    private int m_ColCount = 0;

    private float m_DisableTimer;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    // Trả về true nếu sensor đang hoạt động VÀ có va chạm
    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Tùy chọn: Có thể thêm kiểm tra tag/layer tại đây (ví dụ: other.CompareTag("Ground"))
        m_ColCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        m_ColCount--;
    }

    void Update()
    {
        // Giảm thời gian vô hiệu hóa
        m_DisableTimer -= Time.deltaTime;
        if (m_DisableTimer < 0) 
            m_DisableTimer = 0; // Đảm bảo không âm
    }

    // Hàm vô hiệu hóa tạm thời (dùng sau khi nhảy)
    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}