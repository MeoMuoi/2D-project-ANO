using UnityEngine;

public class BetterJump : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Better Jump Settings")]
    public float fallMultiplier = 2.5f;   // tốc độ rơi nhanh hơn
    public float lowJumpMultiplier = 2f;  // nhấn nhảy ngắn thì rơi sớm

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
