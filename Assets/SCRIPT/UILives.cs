using UnityEngine;
using UnityEngine.UI;

public class UILives : MonoBehaviour
{
    // Kéo thả Component Image từ UI hiển thị mạng vào đây
    public Image livesImage; 
    
    // Mảng Sprite: Element 0 = 0 mạng, Element 1 = 1 mạng, ...
    public Sprite[] lifeSprites; 

    private int maxLives;

    public void SetMaxLives(int max)
    {
        maxLives = max;
        // Tùy chọn: SetLives(max) nếu bạn muốn hiển thị full mạng khi game bắt đầu.
    }

    public void SetLives(int current, int max) // overload 2 tham số
    {
        maxLives = max;
        SetLives(current);
    }

    public void SetLives(int current) // 1 tham số
    {
        if (lifeSprites.Length == 0 || livesImage == null) return;

        // ✅ LOGIC ĐÃ SỬA: Dùng Clamp để đảm bảo index nằm trong phạm vi mảng.
        // Index 0 (lifeSprites[0]) là 0 mạng, Index 1 là 1 mạng, v.v.
        int index = Mathf.Clamp(current, 0, lifeSprites.Length - 1);
        
        livesImage.sprite = lifeSprites[index];
    }
}