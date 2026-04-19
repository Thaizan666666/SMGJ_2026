using UnityEngine;
using static EffectEnums;

[System.Serializable]
public class PassiveStatModifier
{
    [Header("Condition")]
    [Tooltip("Which stat to watch as the trigger")]
    public PassiveWatchStat watchStat;

    public ThresholdMode mode;

    [Tooltip("Primary threshold (0.70 = 70% of max for gauge, or flat value for Speed)")]
    public float thresholdValue = 0.70f;

    [Tooltip("Only used when mode is Between — upper bound (must be > thresholdValue)")]
    public float thresholdValueMax = 1f;

    [Header("Modifier applied when condition is met")]
    public TimedStatType affectStat;

    [Tooltip("Flat units added to the stat")]
    public float flatBonus;

    [Range(-1f, 5f)]
    [Tooltip("0.20 = +20%,  -0.30 = -30%")]
    public float percentBonus;
}