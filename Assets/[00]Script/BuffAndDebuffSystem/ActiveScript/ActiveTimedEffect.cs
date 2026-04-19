public class ActiveTimedEffect
{
    public TimedEffectData Data;
    public float RemainingTime;

    public ActiveTimedEffect(TimedEffectData data)
    {
        Data = data;
        RemainingTime = data.duration;
    }
}