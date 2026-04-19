using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float m_MaxSpeed;
    private float m_Acceleration;
    private float m_Deceleration;
    private float m_SkidDeceleration;

    [Header("Stun Settings")]
    [SerializeField] public bool _IsHit = false;
    [SerializeField] public float cd = 3f;
    private float _cdstun = 0f;

    private Rigidbody2D m_Rb;
    private InputSystem m_Input;
    private CharacterStats m_CharacterStats;
    private MinigameCrab m_MinigameCrab;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Input = GetComponent<InputSystem>();
        m_CharacterStats = GetComponent<CharacterStats>();
        m_MinigameCrab = FindFirstObjectByType<MinigameCrab>();
        if (m_MinigameCrab == null)
        {
            m_MinigameCrab = new MinigameCrab();
        }
        RelinkStats();
    }

    // Pull current post-modifier values from CharacterStats each physics tick
    public void RelinkStats()
    {
        m_MaxSpeed = m_CharacterStats.currentMaxSpeed;
        m_Acceleration = m_CharacterStats.currentAcceleration;
        m_Deceleration = m_CharacterStats.currentDeceleration;
        m_SkidDeceleration = m_CharacterStats.currentSkidDeceleration;
    }

    void FixedUpdate()
    {
        RelinkStats();   // always read latest modded values first

        if (CoditionMove())
            HandleMovement();
        else
            TickStun();
    }

    private void HandleMovement()
    {
        float inputX = m_Input.InputVector.x;
        float velocityX = m_Rb.linearVelocity.x;
        float targetSpeed = inputX * m_MaxSpeed;

        float accel;
        if (inputX == 0)
            accel = m_Deceleration;
        else if (Mathf.Sign(inputX) != Mathf.Sign(velocityX) && velocityX != 0)
            accel = m_SkidDeceleration;
        else
            accel = m_Acceleration;

        float newX = Mathf.MoveTowards(velocityX, targetSpeed, accel * Time.fixedDeltaTime);
        m_Rb.linearVelocity = new Vector2(newX, m_Rb.linearVelocity.y);

        // Store actual physics velocity separately from the max speed cap
        m_CharacterStats.currentVelocityX = newX;

        Debug.LogWarning($"MaxSpeed: {m_MaxSpeed}");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball") && !_IsHit)
        {
            _IsHit = true;
            _cdstun = 0f;
            // Zero out horizontal velocity immediately on stun hit
            m_Rb.linearVelocity = new Vector2(0f, m_Rb.linearVelocity.y);
            Debug.Log("Stunned");
        }
    }

    private void TickStun()
    {
        // Smoothly bring the player to a stop while stunned
        m_Rb.linearVelocity = new Vector2(
            Mathf.MoveTowards(m_Rb.linearVelocity.x, 0f, m_Deceleration * Time.fixedDeltaTime),
            m_Rb.linearVelocity.y
        );

        _cdstun += Time.fixedDeltaTime;   

        if (_cdstun >= cd)
        {
            _IsHit = false;
            _cdstun = 0f;
            Debug.Log("Stun ended — can move");
        }
    }


    // Force a stun from outside (e.g. ObstacleDebuff, damage system)
    public void ApplyStun(float duration)
    {
        _IsHit = true;
        _cdstun = 0f;
        cd = duration;
        m_Rb.linearVelocity = new Vector2(0f, m_Rb.linearVelocity.y);
    }

    public bool CoditionMove()
    {
        if (_IsHit || m_MinigameCrab.isActive)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Read stun state from UI or other systems
    public bool IsStunned => _IsHit;
}