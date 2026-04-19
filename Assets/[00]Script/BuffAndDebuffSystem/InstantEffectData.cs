using UnityEngine;
using static EffectEnums;

[CreateAssetMenu(menuName = "Effects/InstantEffect")]
public class InstantEffectData : ScriptableObject
{
    public string effectId;
    public string displayName;
    public InstantStatType stat;
    public ModifierMode mode;
    public float flatValue;
}