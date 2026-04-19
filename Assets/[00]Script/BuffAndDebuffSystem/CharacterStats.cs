using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Movement — Base Values")]
    public float MaxSpeed = 5f;
    public float Acceleration = 20f;
    public float Deceleration = 15f;
    public float SkidDeceleration = 30f;

    [HideInInspector] public float currentMaxSpeed;
    [HideInInspector] public float currentAcceleration;
    [HideInInspector] public float currentDeceleration;
    [HideInInspector] public float currentSkidDeceleration;
    [HideInInspector] public float currentVelocityX;

    [Header("Heat — Base Values")]
    public float baseHeatIncreaseRate = 5f;
    public float baseHeatDecreaseRate = 3f;

    [HideInInspector] public float currentHeatIncreaseRate;

    [Header("Heat Gauge")]
    public float maxHeatGauge = 100f;
    [HideInInspector] public float currentHotGauge;  // mirror — written by HotGauge

    void Awake()
    {
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
    }

    // Permanently shifts MaxSpeed base (instant effect, no undo)
    public void ApplyInstantSpeed(float delta)
    {
        MaxSpeed = Mathf.Max(0f, MaxSpeed + delta);
        RecalculateTimed(0f, 0f, 0f, 0f);
    }

    // Instant HeatGauge delta — delegates to HotGauge so it stays owner of the value
    public void ApplyInstantHeatGauge(float delta)
    {
        var hotGauge = GetComponent<HotGauge>();
        if (hotGauge != null)
            hotGauge.AddToGauge(delta);
        else
            currentHotGauge = Mathf.Clamp(currentHotGauge + delta, 0f, maxHeatGauge);
    }
}