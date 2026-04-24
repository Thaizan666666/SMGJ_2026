using UnityEngine;
using UnityEngine.Events;

public class IceCreamCount : MonoBehaviour
{
    [Header("Setting Count Times")]
    public float totalTime = 90f;

    [Header("Events")]
    public UnityEvent onTimerStart;
    public UnityEvent onTimerEnd;

    [Header("UI")]
    [SerializeField] private GameObject BTN_E;

    public float CurrentTime => currentTime;
    public float NormalizedTime => currentTime / totalTime;
    public bool HasStarted => hasStarted;
    public bool IsFinished => isFinished;

    private float currentTime;
    private bool hasStarted = false;
    private bool isFinished = false;
    private bool isPlayerInZone = false;

    private void Start()
    {
        currentTime = totalTime;
        BTN_E?.SetActive(false);
    }

    private void Update()
    {
        // Start the timer when player is in zone and presses E
        if (!hasStarted && isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            hasStarted = true;
            BTN_E?.SetActive(false);
            onTimerStart?.Invoke();
            Debug.Log("เริ่มนับถอยหลัง!");
        }

        if (!hasStarted || isFinished) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isFinished = true;
            Debug.Log("End");
            onTimerEnd?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInZone = true;

        // Only show the button if the timer hasn't started yet
        if (!hasStarted && !isFinished)
            BTN_E?.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInZone = false;
        BTN_E?.SetActive(false);
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        hasStarted = false;
        isFinished = false;

        // Show button again if player is still standing in the zone
        if (isPlayerInZone)
            BTN_E?.SetActive(true);
    }
}