using UnityEngine;
using UnityEngine.UI;

public class UILives : MonoBehaviour
{
    public Image livesImage;
    public Sprite[] lifeSprites; // sprite mạng

    private int maxLives;

    public void SetMaxLives(int max)
    {
        maxLives = max;
    }

    public void SetLives(int current, int max) // overload 2 tham số
    {
        maxLives = max;
        SetLives(current);
    }

    public void SetLives(int current) // 1 tham số
    {
        if (lifeSprites.Length == 0 || livesImage == null) return;

        int index = Mathf.Clamp(current, 0, lifeSprites.Length - 1);
        livesImage.sprite = lifeSprites[index];
    }
}
