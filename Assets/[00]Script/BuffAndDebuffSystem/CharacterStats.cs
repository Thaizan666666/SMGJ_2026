using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Movement — Base Values")]
    public float MaxSpeed = 5f;
    public float Acceleration = 20f;
    public float Deceleration = 15f;
    public float SkidDeceleration = 30f;

    [Header("Minimum Caps — prevent zero or negative")]
    [Tooltip("Slowest the player can ever move, even with heavy debuffs")]
    public float MinSpeed = 1f;
    [Tooltip("Lowest heat increase rate allowed, even with debuffs")]
    public float MinHeatIncreaseRate = 0.5f;

    // Runtime movement stats — read by PlayerMovement via RelinkStats
    [HideInInspector] public float currentMaxSpeed;
    [HideInInspector] public float currentAcceleration;
    [HideInInspector] public float currentDeceleration;
    [HideInInspector] public float currentSkidDeceleration;
    [HideInInspector] public float currentVelocityX;   // actual physics velocity, written by PlayerMovement

    [Header("Heat — Base Values")]
    public float baseHeatIncreaseRate = 5f;
    public float baseHeatDecreaseRate = 3f;

    // Runtime heat stats — read by HotGauge via RelinkStats
    [HideInInspector] public float currentHeatIncreaseRate;

    [Header("Heat Gauge")]
    public float maxHeatGauge = 100f;
    [HideInInspector] public float currentHotGauge;   // mirror — written by HotGauge each frame

    // ── Stored modifier layers ────────────────────────────────────────────
    // Timed layer (EffectManager)
    private float _timedSpeedFlat = 0f, _timedSpeedPct = 0f;
    private float _timedAccelFlat = 0f, _timedAccelPct = 0f;
    private float _timedDecelFlat = 0f, _timedDecelPct = 0f;
    private float _timedSkidFlat = 0f, _timedSkidPct = 0f;
    private float _timedHeatFlat = 0f, _timedHeatPct = 0f;

    // Passive layer (PassiveBuffHandler)
    private float _passiveSpeedFlat = 0f, _passiveSpeedPct = 0f;
    private float _passiveAccelFlat = 0f, _passiveAccelPct = 0f;
    private float _passiveDecelFlat = 0f, _passiveDecelPct = 0f;
    private float _passiveSkidFlat = 0f, _passiveSkidPct = 0f;
    private float _passiveHeatRateFlat = 0f, _passiveHeatRatePct = 0f;

    void Awake()
    {
        ApplyAll();
    }

    // ── Called by EffectManager whenever timed modifiers change ──────────
    public void RecalculateTimed(StatModifierBundle timed)
    {
        _timedSpeedFlat = timed.speedFlat; _timedSpeedPct = timed.speedPct;
        _timedAccelFlat = timed.accelFlat; _timedAccelPct = timed.accelPct;
        _timedDecelFlat = timed.decelFlat; _timedDecelPct = timed.decelPct;
        _timedSkidFlat = timed.skidFlat; _timedSkidPct = timed.skidPct;
        _timedHeatFlat = timed.heatFlat; _timedHeatPct = timed.heatPct;
        ApplyAll();
    }

    // ── Called by PassiveBuffHandler whenever threshold state changes ─────
    public void ApplyPassiveModifiers(StatModifierBundle passive)
    {
        _passiveSpeedFlat = passive.speedFlat; _passiveSpeedPct = passive.speedPct;
        _passiveAccelFlat = passive.accelFlat; _passiveAccelPct = passive.accelPct;
        _passiveDecelFlat = passive.decelFlat; _passiveDecelPct = passive.decelPct;
        _passiveSkidFlat = passive.skidFlat; _passiveSkidPct = passive.skidPct;
        _passiveHeatRateFlat = passive.heatFlat; _passiveHeatRatePct = passive.heatPct;
        ApplyAll();
    }

    // ── Combines both layers into current runtime values ─────────────────
    private void ApplyAll()
    {
        float rawSpeed = (MaxSpeed + _timedSpeedFlat + _passiveSpeedFlat)
                         * (1f + _timedSpeedPct + _passiveSpeedPct);
        currentMaxSpeed = Mathf.Max(rawSpeed, MinSpeed);

        float rawAccel = (Acceleration + _timedAccelFlat + _passiveAccelFlat)
                         * (1f + _timedAccelPct + _passiveAccelPct);
        currentAcceleration = Mathf.Max(rawAccel, 0.1f);

        float rawDecel = (Deceleration + _timedDecelFlat + _passiveDecelFlat)
                         * (1f + _timedDecelPct + _passiveDecelPct);
        currentDeceleration = Mathf.Max(rawDecel, 0.1f);

        float rawSkid = (SkidDeceleration + _timedSkidFlat + _passiveSkidFlat)
                        * (1f + _timedSkidPct + _passiveSkidPct);
        currentSkidDeceleration = Mathf.Max(rawSkid, 0.1f);

        float rawHeatRate = (baseHeatIncreaseRate + _timedHeatFlat + _passiveHeatRateFlat)
                            * (1f + _timedHeatPct + _passiveHeatRatePct);
        currentHeatIncreaseRate = Mathf.Max(rawHeatRate, MinHeatIncreaseRate);
    }

    // ── Instant one-shot writes (permanent, no undo) ─────────────────────

    // Permanently shifts MaxSpeed base so future recalculations include it
    public void ApplyInstantSpeed(float delta)
    {
        MaxSpeed = Mathf.Max(0f, MaxSpeed + delta);
        ApplyAll();
    }

    // Delegates to HotGauge so it stays the owner of the gauge value
    public void ApplyInstantHeatGauge(float delta)
    {
        var hotGauge = GetComponent<HotGauge>();
        if (hotGauge != null)
            hotGauge.AddToGauge(delta);
        else
            currentHotGauge = Mathf.Clamp(currentHotGauge + delta, 0f, maxHeatGauge);
    }
}