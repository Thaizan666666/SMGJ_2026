using static EffectEnums;

public struct StatModifierBundle
{
    public float speedFlat, speedPct;
    public float accelFlat, accelPct;
    public float decelFlat, decelPct;
    public float skidFlat, skidPct;
    public float heatFlat, heatPct;

    // Accumulate one TimedStatEntry into this bundle
    public void Add(TimedStatEntry entry)
    {
        float sign = entry.mode == ModifierMode.Add ? 1f : -1f;
        float flat = entry.flatValue * sign;
        float pct = entry.percentValue * sign;

        switch (entry.stat)
        {
            case TimedStatType.Speed: speedFlat += flat; speedPct += pct; break;
            case TimedStatType.Acceleration: accelFlat += flat; accelPct += pct; break;
            case TimedStatType.Deceleration: decelFlat += flat; decelPct += pct; break;
            case TimedStatType.SkidDeceleration: skidFlat += flat; skidPct += pct; break;
            case TimedStatType.HeatIncreaseRate: heatFlat += flat; heatPct += pct; break;
        }
    }

    // Accumulate a passive rule's bonus into this bundle
    public void AddPassive(PassiveStatModifier rule)
    {
        switch (rule.affectStat)
        {
            case TimedStatType.Speed: speedFlat += rule.flatBonus; speedPct += rule.percentBonus; break;
            case TimedStatType.Acceleration: accelFlat += rule.flatBonus; accelPct += rule.percentBonus; break;
            case TimedStatType.Deceleration: decelFlat += rule.flatBonus; decelPct += rule.percentBonus; break;
            case TimedStatType.SkidDeceleration: skidFlat += rule.flatBonus; skidPct += rule.percentBonus; break;
            case TimedStatType.HeatIncreaseRate: heatFlat += rule.flatBonus; heatPct += rule.percentBonus; break;
        }
    }
}