using UnityEngine;
using System.Collections;

public class MainCharacter : MonoBehaviour
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

    [Header("Invincibility")]
    [SerializeField] private float m_invincibilityDuration = 1.0f;

    [Header("Attack Zone")]
    // Kéo thả GameObject chứa script PlayerAttackZone vào đây
    public GameObject attackZone; 

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    // Đã sửa lỗi: Component phải là Sensor_MainCharacter
    private Sensor_MainCharacter m_groundSensor; 

    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;

    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool m_isInvincible = false;
    // Public property để GameManager kiểm tra I-frames
    public bool IsInvincible => m_isInvincible; 
    private bool canControl = true;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();

        // LỖI ĐÃ ĐƯỢC SỬA: Tìm component Sensor_MainCharacter
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_MainCharacter>(); 
        
        if (m_groundSensor == null)
        {
            Debug.LogError("LỖI KHỞI TẠO: Không tìm thấy Component 'Sensor_MainCharacter' trên GameObject con tên 'GroundSensor'!");
        }
    }

    void Update()
    {
        if (!canControl)
        {
            m_body2d.linearVelocity = Vector2.zero;
            m_animator.SetInteger("AnimState", 0);
            return;
        }
        
        // Kiểm tra an toàn m_groundSensor để tránh NullReferenceException
        if (m_groundSensor == null)
        {
            Debug.LogWarning("GroundSensor bị thiếu! Không thể kiểm tra mặt đất.");
            return;
        }

        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling) m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration) m_rolling = false;

        // Ground check
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", true);
        }
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
        }

        // Coyote time
        if (m_grounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        // Jump buffer
        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCounter = jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        float inputX = Input.GetAxis("Horizontal");

        // Flip
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

        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0;
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

            m_body2d.linearVelocity = new Vector2(
                m_facingDirection * m_rollForce,
                m_body2d.linearVelocity.y
            );
        }

        // Jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", false);

            m_body2d.linearVelocity = new Vector2(
                m_body2d.linearVelocity.x,
                m_jumpForce
            );

            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;

            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(inputX) > 0.01f)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle <= 0)
                m_animator.SetInteger("AnimState", 0);
        }

        // Better jump
        if (m_body2d.linearVelocity.y < 0)
        {
            m_body2d.linearVelocity += Vector2.up *
                Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (m_body2d.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            m_body2d.linearVelocity += Vector2.up *
                Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // ================================
    // Damage handling
    // ================================
    public void TriggerHurt()
    {
        if (m_rolling || m_isInvincible)
            return;

        m_animator.SetTrigger("Hurt");
        StartCoroutine(HandleInvincibility());
    }

    private IEnumerator HandleInvincibility()
    {
        m_isInvincible = true;
        yield return new WaitForSeconds(m_invincibilityDuration);
        m_isInvincible = false;
    }

    // ================================
    // GameManager calls this
    // ================================
    public void SetControl(bool state)
    {
        canControl = state;

        if (!state)
        {
            m_body2d.linearVelocity = Vector2.zero;
            m_animator.SetInteger("AnimState", 0);
        }
    }

    // ================================ 
    // Animation Attack Events (Dùng để bật/tắt vùng tấn công)
    // ================================
    
    // AE_AttackStart() được gọi khi hoạt ảnh tấn công bắt đầu
    void AE_AttackStart() 
    {
        if (attackZone != null)
        {
            attackZone.SetActive(true);
        }
    }

    // AE_AttackEnd() được gọi khi hoạt ảnh tấn công kết thúc
    void AE_AttackEnd() 
    {
        if (attackZone != null)
        {
            attackZone.SetActive(false);
        }
    }
    
    // ================================
    // Animation Event (ĐÃ CÓ SẴN, chỉ giữ lại một bản)
    // ================================
    void AE_SlideDust()
    {
        if (m_slideDust != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.down * 0.5f;
            GameObject dust = Instantiate(m_slideDust, spawnPosition, transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}