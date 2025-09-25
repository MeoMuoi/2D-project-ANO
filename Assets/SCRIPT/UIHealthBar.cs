using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image healthImage;          // chỗ hiển thị sprite
    public Sprite[] healthSprites;     // sprite 0%, 20%, 40%, 60%, 80%, 100%...

    public void SetHealth(int current, int max)
    {
        if (healthSprites.Length == 0 || healthImage == null) return;

        int index = Mathf.Clamp(
            Mathf.RoundToInt((float)current / max * (healthSprites.Length - 1)),
            0,
            healthSprites.Length - 1
        );

        healthImage.sprite = healthSprites[index];
    }
}
