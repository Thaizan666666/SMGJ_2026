using UnityEngine;
using UnityEngine.UI;

public class MinigameCrab : MonoBehaviour
{
    [Header("Settings")]
    public float fillAmount = 5f;
    public float drainSpeed = 0f;
    public float maxValue = 100f;

    [Header("UI")]
    public Slider barSlider;
    public GameObject minigamePanel;

    [Header("Key Indicators")]
    [SerializeField] private RectTransform keyA_Transform;
    [SerializeField] private RectTransform keyD_Transform;
    [SerializeField] private CanvasGroup keyA_CanvasGroup;
    [SerializeField] private CanvasGroup keyD_CanvasGroup;

    [Header("Key Scale Settings")]
    [SerializeField] private float activeScale = 1.2f;   // scale when this key is next
    [SerializeField] private float inactiveScale = 0.85f;  // scale when waiting for other key
    [SerializeField] private float inactiveAlpha = 0.5f;   // opacity when not the active key
    [SerializeField] private float scaleSpeed = 8f;     // lerp speed for smooth transition

    private float currentValue = 0f;
    private bool waitingForD = false;

    public bool isActive { get; private set; }
    public bool isFinish = false;

    // ── Target scale per key, updated each frame ───────────────────────────
    private Vector3 m_TargetScaleA;
    private Vector3 m_TargetScaleD;

    // ─────────────────────────────────────────────────────────────────────

    private void Start()
    {
        barSlider.maxValue = maxValue;
        barSlider.value = currentValue;

        RefreshKeyVisuals(instant: true);
    }

    private void Update()
    {
        if (!isActive) return;

        HandleDrain();
        HandleInput();

        barSlider.value = currentValue;

        RefreshKeyVisuals(instant: false);

        if (currentValue >= maxValue)
            OnBarFull();
    }

    // ── Input ──────────────────────────────────────────────────────────────

    private void HandleDrain()
    {
        currentValue = Mathf.Clamp(currentValue - drainSpeed * Time.deltaTime, 0f, maxValue);
    }

    private void HandleInput()
    {
        if (!waitingForD)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentValue = Mathf.Clamp(currentValue + fillAmount, 0f, maxValue);
                waitingForD = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentValue = Mathf.Clamp(currentValue + fillAmount, 0f, maxValue);
                waitingForD = false;
            }
        }
    }

    // ── Key visuals ────────────────────────────────────────────────────────

    /// <summary>
    /// Updates scale and alpha for both key indicators.
    /// Pass instant = true to snap immediately (no lerp), used on Start / StartMinigame.
    /// </summary>
    private void RefreshKeyVisuals(bool instant)
    {
        // A is active when waitingForD == false (we're waiting for A)
        bool aIsActive = !waitingForD;

        float scaleA = aIsActive ? activeScale : inactiveScale;
        float scaleD = aIsActive ? inactiveScale : activeScale;
        float alphaA = aIsActive ? 1f : inactiveAlpha;
        float alphaD = aIsActive ? inactiveAlpha : 1f;

        if (instant)
        {
            if (keyA_Transform != null) keyA_Transform.localScale = Vector3.one * scaleA;
            if (keyD_Transform != null) keyD_Transform.localScale = Vector3.one * scaleD;
            if (keyA_CanvasGroup != null) keyA_CanvasGroup.alpha = alphaA;
            if (keyD_CanvasGroup != null) keyD_CanvasGroup.alpha = alphaD;
        }
        else
        {
            float t = scaleSpeed * Time.deltaTime;

            if (keyA_Transform != null)
                keyA_Transform.localScale = Vector3.Lerp(
                    keyA_Transform.localScale, Vector3.one * scaleA, t);

            if (keyD_Transform != null)
                keyD_Transform.localScale = Vector3.Lerp(
                    keyD_Transform.localScale, Vector3.one * scaleD, t);

            if (keyA_CanvasGroup != null)
                keyA_CanvasGroup.alpha = Mathf.Lerp(keyA_CanvasGroup.alpha, alphaA, t);

            if (keyD_CanvasGroup != null)
                keyD_CanvasGroup.alpha = Mathf.Lerp(keyD_CanvasGroup.alpha, alphaD, t);
        }
    }

    // ── Finish ─────────────────────────────────────────────────────────────

    private void OnBarFull()
    {
        isActive = false;
        isFinish = true;
        minigamePanel.SetActive(false);
        Debug.Log("หลอดเต็ม! Minigame สำเร็จ");
    }

    // ── Public API ─────────────────────────────────────────────────────────

    public void StartMinigame()
    {
        currentValue = 0f;
        waitingForD = false;
        isFinish = false;
        isActive = true;
        minigamePanel.SetActive(true);
        RefreshKeyVisuals(instant: true);
    }
}