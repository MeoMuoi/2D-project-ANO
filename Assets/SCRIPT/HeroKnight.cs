using UnityEngine;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] GameObject m_slideDust;

    [Header("Better Jump Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Coyote Time & Jump Buffer")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // Jump helpers
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // ✅ Thêm control lock
    private bool canControl = true;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
    }

    void Update()
    {
        if (!canControl)
        {
            // disable toàn bộ input + dừng nhân vật
            m_body2d.linearVelocity = Vector2.zero;
            m_animator.SetInteger("AnimState", 0);
            return;
        }

        // Timers
        m_timeSinceAttack += Time.deltaTime;
        if (m_rolling) m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration) m_rolling = false;

        // Ground check
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // --- Coyote Time ---
        if (m_grounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        // --- Jump Buffer ---
        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        // Input
        float inputX = Input.GetAxis("Horizontal");

        // Flip sprite
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        // AirSpeed
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;
            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }

        // Block
        if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        if (Input.GetKeyDown(KeyCode.LeftShift) && !m_rolling)
        {
            m_rolling = true;
            m_rollCurrentTime = 0;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        }

        // --- Jump ---
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);

            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            m_groundSensor.Disable(0.2f);
        }

        // Run / Idle
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0) m_animator.SetInteger("AnimState", 0);
        }

        // Better jump multipliers
        if (m_body2d.linearVelocity.y < 0)
        {
            m_body2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (m_body2d.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            m_body2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // --- Called from GameManager ---
    public void TriggerHurt()
    {
        if (m_animator != null) m_animator.SetTrigger("Hurt");
    }

    public void TriggerDeath()
    {
        if (m_animator != null) m_animator.SetTrigger("Death");
    }

    // ✅ Thêm SetControl để GameManager gọi
    public void SetControl(bool state)
    {
        canControl = state;
        if (!state)
        {
            m_body2d.linearVelocity = Vector2.zero;
            m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    void AE_SlideDust()
    {
        if (m_slideDust != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.down * 0.5f;
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
