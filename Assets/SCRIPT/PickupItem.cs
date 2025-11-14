using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Health, ExtraLife, Coin, PowerUp }
    public ItemType itemType;
    public int amount = 1;

    [Header("Power Up Settings")]
    public string powerUpID = "";

    [Header("Effects")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    public void OnPickup(GameObject player)
    {
        switch (itemType)
        {
            case ItemType.Health:
                GameManager.Instance.Heal(amount);
                break;

            case ItemType.ExtraLife:
                GameManager.Instance.AddLife(amount);
                break;

            case ItemType.Coin:
                PlayerInventory inv = player.GetComponent<PlayerInventory>();
                if (inv != null) inv.AddCoins(amount);
                break;

            case ItemType.PowerUp:
                PlayerInventory pi = player.GetComponent<PlayerInventory>();
                if (pi != null) pi.PowerUp(powerUpID, amount);
                break;
        }

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        MainCharacter playerScript = other.GetComponent<MainCharacter>();
        if (playerScript != null)
            OnPickup(other.gameObject);
    }
}
