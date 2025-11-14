using UnityEngine;
using System.Collections;

public class PlayerInventory : MonoBehaviour
{
    // BẠN CÓ THỂ THÊM UI CHO COIN VÀ POWERUP TẠI ĐÂY
    [Header("Inventory")]
    public int currentCoins = 0;

    // Ví dụ về PowerUp: Tăng tốc độ
    [Header("Power Up")]
    public float powerUpDuration = 0f;
    private float defaultSpeed;

    void Start()
    {
        // Giả định MainCharacter có m_speed public để tham chiếu
        MainCharacter mc = GetComponent<MainCharacter>();
        if (mc != null)
        {
            // TẠM THỜI BỎ QUA DO m_speed LÀ private TRONG MainCharacter
            // defaultSpeed = mc.m_speed; 
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("Coin Count: " + currentCoins);
        // TODO: Cập nhật UI Coin
    }

    // Hàm PowerUp gọi từ PickupItem.cs
    public void PowerUp(string id, int amount)
    {
        Debug.Log($"Picked up PowerUp: {id} for {amount} seconds/power.");

        if (id == "SpeedBoost")
        {
            // Tạm thời bỏ qua logic tăng tốc độ do m_speed là private
            // TODO: Tìm cách thay đổi m_speed (ví dụ: tạo hàm public SetSpeed trong MainCharacter)
            
            // Xử lý thời gian PowerUp
            if (powerUpDuration > 0)
            {
                // Nếu đang có PowerUp, reset thời gian
                StopCoroutine("PowerUpTimer");
            }
            powerUpDuration = amount;
            StartCoroutine("PowerUpTimer");
        }
    }

    IEnumerator PowerUpTimer()
    {
        // TẠM THỜI: BẠN CẦN LÀM public m_speed trong MainCharacter.cs để dùng logic này
        // MainCharacter mc = GetComponent<MainCharacter>();
        // mc.m_speed *= 2f; // Ví dụ tăng gấp đôi tốc độ

        yield return new WaitForSeconds(powerUpDuration);

        // mc.m_speed = defaultSpeed; // Trả lại tốc độ cũ
        Debug.Log("PowerUp HẾT HẠN!");
        powerUpDuration = 0;
    }
}