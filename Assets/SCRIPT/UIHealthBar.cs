using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image healthImage;
    public Sprite[] healthSprites; // sprite theo từng vạch máu

    private int maxHealth;

    public void SetMaxHealth(int max)
    {
        maxHealth = max;
    }

    public void SetHealth(int current, int max)  // overload 2 tham số
    {
        maxHealth = max;
        SetHealth(current);
    }

    public void SetHealth(int current) // 1 tham số
    {
        if (healthSprites.Length == 0 || healthImage == null) return;

        int index = Mathf.Clamp(Mathf.RoundToInt((float)current / maxHealth * (healthSprites.Length - 1)), 0, healthSprites.Length - 1);
        healthImage.sprite = healthSprites[index];
    }
}
