using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetHealth(int health)
    {
        if (slider != null) slider.value = health;
    }
}
