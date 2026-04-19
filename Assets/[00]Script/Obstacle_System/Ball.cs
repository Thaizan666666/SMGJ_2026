using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float arcHeight = 3f;         // ความสูงของโค้ง Projectile
    public float destroyDelay = 5f;      // ลบหลังจากกี่วินาที

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 targetPos)
    {
        Vector2 velocity = CalculateLaunchVelocity(transform.position, targetPos, arcHeight);
        rb.linearVelocity = velocity;

        Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// คำนวณ Velocity เพื่อให้บอลพุ่งเป็นโค้งไปยังเป้าหมาย
    /// </summary>
    Vector2 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float h)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;

        float dx = target.x - origin.x;
        float dy = target.y - origin.y;

        // เวลาขาขึ้นถึงจุดสูงสุด: h = 0.5 * g * t1^2
        float t1 = Mathf.Sqrt(2f * h / gravity);

        // เวลาขาลงจากจุดสูงสุดถึง target
        // dy = h - 0.5 * g * t2^2  =>  t2 = sqrt(2*(h - dy) / g)
        float t2 = Mathf.Sqrt(2f * (h - dy) / gravity);

        float totalTime = t1 + t2;

        float vx = dx / totalTime;
        float vy = gravity * t1;       // vy ตอน launch = g * t1

        return new Vector2(vx, vy);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Player!");
        }

        if (col.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}