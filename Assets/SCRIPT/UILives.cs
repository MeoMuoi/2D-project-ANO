using UnityEngine;
using UnityEngine.UI;

public class UILives : MonoBehaviour
{
    [SerializeField] private Text livesText;

    public void SetLives(int lives)
    {
        if (livesText != null) livesText.text = "Lives: " + lives;
    }
}
