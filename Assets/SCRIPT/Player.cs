using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Jump")]
    public int maxJumps = 2; // double jump

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private int jumpCount = 0;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- Move ---
        float moveInput = Input.GetAxisRaw("Horizontal"); // A/D or ←/→
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Flip bằng SpriteRenderer (không thay đổi transform.scale) ---
        if (moveInput > 0f) sr.flipX = false;    // mặt phải
        else if (moveInput < 0f) sr.flipX = true; // mặt trái

        // --- Animation chạy/idle ---
        if (animator != null) animator.SetBool("isRunning", moveInput != 0f);

        // --- Nhảy (Space hoặc W), chỉ đếm số lần nhấn ---
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            if (jumpCount < maxJumps)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount++;
                isGrounded = false;

                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                    animator.SetBool("isJumping", true);
                }
            }
        }
    }

    // Reset jump khi chạm đất bằng Tag "Ground"
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            if (animator != null) animator.SetBool("isJumping", false);
        }
    }
}
