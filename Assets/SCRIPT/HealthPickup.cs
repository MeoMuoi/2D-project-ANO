using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // player phải có tag Player
        {
            HeroKnight hero = collision.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.Heal(1); // luôn luôn +1 máu

                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);

                Destroy(gameObject); // ăn xong thì biến mất
            }
        }
    }
}
