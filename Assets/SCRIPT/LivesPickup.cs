using UnityEngine;

public class LivesPickup : MonoBehaviour
{
    [SerializeField] private int lifeAmount = 1; // số mạng hồi thêm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HeroKnight player = collision.GetComponent<HeroKnight>();
        if (player != null)
        {
            GameManager.Instance.AddLife(lifeAmount);
            Destroy(gameObject);
        }
    }
}
