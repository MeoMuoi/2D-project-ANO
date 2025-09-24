using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;

    public void SetHealth(int currentHealth)
    {
        // clamp index
        int index = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);
        healthBarImage.sprite = healthSprites[index];
    }
}
