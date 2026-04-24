using UnityEngine;
using UnityEngine.InputSystem;
using static ManagerSound;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float m_JumpForce = 12f;
    [SerializeField] private float m_MinJumpTime = 0.1f;   // กดแป้บเดียว = กระโดดต่ำ
    [SerializeField] private float m_MaxJumpTime = 0.35f;  // กดค้าง = กระโดดสูง
    [SerializeField] private float m_HoldGravityScale = 1f;   // gravity ขณะกดค้าง (เบา)
    [SerializeField] private float m_FallGravityScale = 3f;   // gravity ขณะตก (หนัก)
    [SerializeField] private KeyCode m_JumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private float m_GroundCheckRadius = 0.1f;
    [SerializeField] private LayerMask m_GroundLayer;

    private Rigidbody2D m_Rb;
    private bool m_IsGrounded;
    private bool m_IsJumping;
    private float m_JumpTimeCounter;
    private PlayerMovement m_Movement;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CheckGround();
        HandleJumpInput();
        HandleGravity();
    }

    private bool m_WasGrounded;

    private void CheckGround()
    {
        m_IsGrounded = Physics2D.OverlapCircle(
            m_GroundCheck.position,
            m_GroundCheckRadius,
            m_GroundLayer
        );

        // Just landed this frame
        if (m_IsGrounded && !m_WasGrounded)
            PlayEffect("Landing");

        m_WasGrounded = m_IsGrounded;
    }

    private void HandleJumpInput()
    {
        // กดปุ่ม Jump + อยู่บนพื้น → เริ่มกระโดด
        if (Keyboard.current.spaceKey.wasPressedThisFrame && m_IsGrounded && !m_Movement._IsHit)
        {
            m_IsJumping = true;
            PlayEffect("Jump");
            m_JumpTimeCounter = 0f;
            m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, m_JumpForce);
        }

        // กดค้างอยู่ + ยังอยู่ในช่วง MaxJumpTime → เพิ่มแรงขึ้น
        if (Keyboard.current.spaceKey.isPressed && m_IsJumping && !m_Movement._IsHit)
        {
            if (m_JumpTimeCounter < m_MaxJumpTime)
            {
                m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, m_JumpForce);
                m_JumpTimeCounter += Time.deltaTime;
            }
            else
            {
                m_IsJumping = false; // ครบ MaxJumpTime แล้ว หยุดเพิ่มแรง
            }
        }

        // ปล่อยปุ่มก่อนครบเวลา → หยุดกระโดด
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            m_IsJumping = false;
        }
    }

    private void HandleGravity()
    {
        if (m_IsJumping && Keyboard.current.spaceKey.isPressed)
        {
            // กดค้าง → gravity เบา ลอยได้นานขึ้น
            m_Rb.gravityScale = m_HoldGravityScale;
        }
        else
        {
            // ปล่อย หรือ ตกลง → gravity หนัก
            m_Rb.gravityScale = m_FallGravityScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_GroundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundCheckRadius);
    }
}
