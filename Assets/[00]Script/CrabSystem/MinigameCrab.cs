using UnityEngine;
using UnityEngine.UI;

public class MinigameCrab : MonoBehaviour
{
    [Header("Settings")]
    public float fillAmount = 5f;        // เพิ่มต่อครั้งที่กดถูก
    public float drainSpeed = 0f;        // ลดต่อวินาที
    public float maxValue = 100f;        // ค่าสูงสุดของหลอด

    [Header("UI")]
    public Slider barSlider;             // ลาก Slider มาใส่
    public GameObject minigamePanel;     // ลาก Panel UI มาใส่
    private float currentValue = 0f;
    private bool waitingForD = false;    // false = รอกด A, true = รอกด D
    public bool isActive { get; private set; }  // เปิด/ปิด minigame
    public bool isFinish = false;

    void Start()
    {
        barSlider.maxValue = maxValue;
        barSlider.value = currentValue;
    }

    void Update()
    {
        if (!isActive) return;

        HandleDrain();
        HandleInput();

        barSlider.value = currentValue;

        if (currentValue >= maxValue)
        {
            OnBarFull();
        }
    }

    void HandleDrain()
    {
        currentValue -= drainSpeed * Time.deltaTime;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
    }

    void HandleInput()
    {
        if (!waitingForD)
        {
            // รอกด A
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentValue += fillAmount;
                currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
                waitingForD = true;   // ต่อไปรอ D
            }
        }
        else
        {
            // รอกด D
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentValue += fillAmount;
                currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
                waitingForD = false;  // วนกลับรอ A
            }
        }
    }

    void OnBarFull()
    {
        isActive = false;
        isFinish = true;
        minigamePanel.SetActive(false);
        Debug.Log("หลอดเต็ม! Minigame สำเร็จ");
        // ใส่ Logic ต่อที่นี่ เช่น ปิด Panel, เรียก Event
    }

    // เรียกจากภายนอกเพื่อเริ่ม Minigame ใหม่
    public void StartMinigame()
    {
        currentValue = 0f;
        waitingForD = false;
        isFinish = false;
        isActive = true;
        minigamePanel.SetActive(true);
    }
}
