using static EffectEnums;

public class ActiveTimedEffect
{
    public TimedEffectData Data;
    public float RemainingTime;

    public float SignedValue =>
        Data.mode == ModifierMode.Add ? Data.flatValue : -Data.flatValue;

    public ActiveTimedEffect(TimedEffectData data)
    {
        Data = data;          
        RemainingTime = data.duration;
    }
}