using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_MaxSpeed = 8f;
    [SerializeField] private float m_Acceleration = 20f;
    [SerializeField] private float m_Deceleration = 15f;
    [SerializeField] private float m_SkidDeceleration = 30f;

    private Rigidbody2D m_Rb;
    private InputSystem m_Input;

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Input = GetComponent<InputSystem>();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float inputX = m_Input.InputVector.x;
        float velocityX = m_Rb.velocity.x;

        float targetSpeed = inputX * m_MaxSpeed;
        float accel;

        if (inputX == 0)
        {
            // ปล่อยปุ่ม → Decelerate
            accel = m_Deceleration;
        }
        else if (Mathf.Sign(inputX) != Mathf.Sign(velocityX) && velocityX != 0)
        {
            // เปลี่ยนทิศ → Skid
            accel = m_SkidDeceleration;
        }
        else
        {
            // เดินหน้าปกติ → Accelerate
            accel = m_Acceleration;
        }

        float newX = Mathf.MoveTowards(velocityX, targetSpeed, accel * Time.fixedDeltaTime);
        m_Rb.velocity = new Vector2(newX, m_Rb.velocity.y);
    }
}
