using UnityEngine;
using UnityEngine.UI;

public class UILives : MonoBehaviour
{
    public Image livesImage;        // chỗ hiển thị sprite
    public Sprite[] livesSprites;   // sprite cho 0 mạng, 1 mạng, 2 mạng, 3 mạng...

    public void SetLives(int current, int max)
    {
        if (livesSprites.Length == 0 || livesImage == null) return;

        int index = Mathf.Clamp(current, 0, livesSprites.Length - 1);
        livesImage.sprite = livesSprites[index];
    }
}
