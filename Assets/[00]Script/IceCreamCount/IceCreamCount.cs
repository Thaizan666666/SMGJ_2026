using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;

public class IceCreamCount : MonoBehaviour
{
    [Header("Setting Count Times")]
    public float totalTime = 90f;

    [Header("Events")]
    public UnityEvent onTimerStart;   // ถูกเรียกครั้งแรกที่ชน
    public UnityEvent onTimerEnd;     // ถูกเรียกเมื่อหมดเวลา

    public float CurrentTime => currentTime;
    public float NormalizedTime => currentTime / totalTime;
    public bool HasStarted => hasStarted;

    private float currentTime;
    private bool hasStarted = false;
    private bool isFinished = false;
    private bool isPlayerInZone = false;

    void Start()
    {
        currentTime = totalTime;
    }

    private void Update()
    {
        if (!hasStarted && isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            hasStarted = true;
            onTimerStart?.Invoke();
            Debug.Log("เริ่มนับถอยหลัง!");
        }

        if (!hasStarted || isFinished ) { return; }
        
        currentTime -= Time.deltaTime;
        Debug.Log(currentTime);

        if (currentTime <= 0f)
        {
            Debug.Log("End");
            currentTime = 0f;
            isFinished = true;
            onTimerEnd?.Invoke();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInZone = false;
    }
    public void ResetTimer()
    {
        currentTime = totalTime;
        hasStarted = false;
        isFinished = false;
    }
}
