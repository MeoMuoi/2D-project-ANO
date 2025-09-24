using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    [Header("Better Jump Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Coyote Time & Jump Buffer")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    private UIHealthBar uiHealthBar;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // Better Jump extras
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        // Health setup
        currentHealth = maxHealth;
        uiHealthBar = FindObjectOfType<UIHealthBar>();
        if (uiHealthBar != null)
            uiHealthBar.SetHealth(currentHealth);
    }

    void Update()
    {
        // Timers
        m_timeSinceAttack += Time.deltaTime;
        if (m_rolling) m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration) m_rolling = false;

        // Check landing / leaving ground
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
        if (m_grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // --- Jump Buffer ---
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Handle input
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

        // Anim AirSpeed
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // Wall slide check
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) ||
                          (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            TakeDamage(1);
        }
        // Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
        {
            TakeDamage(1);
        }
        // Attack
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;
            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }
        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        }

        // --- Jump (Better Jump + Coyote + Buffer) ---
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);

            // Reset
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            m_groundSensor.Disable(0.2f);
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        // Idle
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }

        // --- Apply Better Jump multipliers ---
        if (m_body2d.linearVelocity.y < 0)
        {
            m_body2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (m_body2d.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            m_body2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // --- TEST Heal (ấn H để hồi 1 máu) ---
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(1);
        }
    }

    // --- Health System ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (uiHealthBar != null)
            uiHealthBar.SetHealth(currentHealth);

        m_animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            m_animator.SetTrigger("Death");
            // disable player control ở đây nếu muốn
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (uiHealthBar != null)
            uiHealthBar.SetHealth(currentHealth);
    }

    // Animation Events
    void AE_SlideDust()
    {
        Vector3 spawnPosition = (m_facingDirection == 1)
            ? m_wallSensorR2.transform.position
            : m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
