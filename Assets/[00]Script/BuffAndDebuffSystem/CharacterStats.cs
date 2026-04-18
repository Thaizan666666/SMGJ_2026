using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Movement — Base Values")]
    public float MaxSpeed = 5f;
    public float Acceleration = 20f;
    public float Deceleration = 15f;
    public float SkidDeceleration = 30f;

    // ── Runtime movement stats (read by PlayerMovement via RelinkStats) ──
    public float currentMaxSpeed;
    [HideInInspector] public float currentAcceleration;
    [HideInInspector] public float currentDeceleration;
    [HideInInspector] public float currentSkidDeceleration;
    [HideInInspector] public float currentVelocityX;   // actual physics velocity, set by PlayerMovement

    [Header("Heat — Base Values")]
    public float baseHeatIncreaseRate = 5f;
    public float baseHeatDecreaseRate = 3f;

    // ── Runtime heat stats ───────────────────────────────────────────────
    [HideInInspector] public float currentHeatIncreaseRate;
    [HideInInspector] public float currentHeatDecreaseRate;

    [Header("Heat Gauge")]
    public float maxHeatGauge = 100f;
    [HideInInspector] public float currentHotGauge;

    private HotGauge m_HotGauge;

    void Awake()
    {
        m_HotGauge = GetComponent<HotGauge>();
        currentHotGauge = m_HotGauge != null ? m_HotGauge.CurrentGauge : 0f;

        // Initialise runtime values to base
        RecalculateTimed(0f, 0f, 0f, 0f);
    }

    public void RecalculateTimed(
        float speedFlat, float speedPercent,
        float heatRateFlat, float heatRatePercent)
    {
        currentMaxSpeed = (MaxSpeed + speedFlat) * (1f + speedPercent);
        currentAcceleration = Acceleration;
        currentDeceleration = Deceleration;
        currentSkidDeceleration = SkidDeceleration;
        currentHeatIncreaseRate = (baseHeatIncreaseRate + heatRateFlat) * (1f + heatRatePercent);
        currentHeatDecreaseRate = baseHeatDecreaseRate;
    }

    // ── Instant one-shot writes (permanent, no undo) ─────────────────────

    public void ApplyInstantSpeed(float delta)
    {
        // Shifts the base permanently so future recalculations include it
        MaxSpeed = Mathf.Max(0f, MaxSpeed + delta);
        RecalculateTimed(0f, 0f, 0f, 0f); 
    }

    public void ApplyInstantHeatGauge(float delta)
    {
        currentHotGauge = Mathf.Clamp(currentHotGauge + delta, 0f, maxHeatGauge);
        if (m_HotGauge != null) m_HotGauge.CurrentGauge = currentHotGauge;
    }
}