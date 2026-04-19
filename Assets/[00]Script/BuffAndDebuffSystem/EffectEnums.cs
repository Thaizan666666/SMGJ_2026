using UnityEngine;

public class EffectEnums 
{
    public enum TimedStatType { Speed, HeatIncreaseRate, Acceleration, Deceleration, SkidDeceleration }
    public enum InstantStatType { Speed, HeatGauge }
    public enum ModifierMode { Add, Subtract }
    public enum PassiveWatchStat { HeatGauge, Speed }
    public enum ThresholdMode { AboveOrEqual, Above, BelowOrEqual, Below, Between }
}
