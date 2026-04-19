using UnityEngine;
using static EffectEnums;

[CreateAssetMenu(menuName = "Effects/TimedEffect")]
public class TimedEffectData : ScriptableObject
{
    public string effectId;
    public string displayName;
    public float duration;

    public TimedStatType stat;
    public ModifierMode mode;

    [Header("Modifier — leave unused field at 0")]
    public float flatValue;        // e.g.  2  = +2 units
    [Range(-1f, 5f)]
    public float percentValue;     // e.g.  0.30 = +30%,  -0.50 = -50%
}
