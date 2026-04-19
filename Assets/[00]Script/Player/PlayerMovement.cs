using JetBrains.Annotations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float m_MaxSpeed;
    private float m_Acceleration;
    private float m_Deceleration;
    private float m_SkidDeceleration;

    private Rigidbody2D m_Rb;
    private InputSystem m_Input;
    private CharacterStats m_CharacterStats;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Input = GetComponent<InputSystem>();
        m_CharacterStats = GetComponent<CharacterStats>();
        RelinkStats();
    }

    // Pull current (post-modifier) values from CharacterStats each physics tick
    public void RelinkStats()
    {
        m_MaxSpeed = m_CharacterStats.currentMaxSpeed;
        m_Acceleration = m_CharacterStats.currentAcceleration;
        m_Deceleration = m_CharacterStats.currentDeceleration;
        m_SkidDeceleration = m_CharacterStats.currentSkidDeceleration;
    }

    void FixedUpdate()
    {
        RelinkStats();       // always read latest modded values first
        HandleMovement();
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

        // Store actual physics velocity separately — not the same as max speed
        m_CharacterStats.currentVelocityX = newX;
        Debug.LogWarning($"Max Speed :{m_MaxSpeed}");
    }
}