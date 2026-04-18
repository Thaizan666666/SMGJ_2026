using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode m_RightKey = KeyCode.D;
    [SerializeField] private KeyCode m_LeftKey = KeyCode.A;

    // Private members
    private Vector2 m_InputVector;
    private float m_XInput;

    public Vector2 InputVector => m_InputVector;

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        m_XInput = 0;
        if (Input.GetKey(m_LeftKey))
        {
            m_XInput--;               //ไปซ้าย
        }
        if (Input.GetKey(m_RightKey))
        {
            m_XInput++;               //ไปขวา
        }
        m_InputVector = new Vector2(m_XInput,0);

        //Debug.Log(m_InputVector);
    }
}
