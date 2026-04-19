using System.Collections.Generic;
using UnityEngine;
using static EffectEnums;

// One stat entry inside a timed effect — add as many as needed
[System.Serializable]
public class TimedStatEntry
{
    public TimedStatType stat;
    public ModifierMode mode;

    [Tooltip("Flat units added/subtracted")]
    public float flatValue;

    [Range(-1f, 5f)]
    [Tooltip("0.30 = +30%,  -0.50 = -50%. Leave 0 if unused.")]
    public float percentValue;
}

[CreateAssetMenu(menuName = "Effects/TimedEffect")]
public class TimedEffectData : ScriptableObject
{
    public string effectId;
    public string displayName;
    public float duration;

    [Tooltip("Add one entry per stat this effect modifies")]
    public List<TimedStatEntry> statEntries = new();
}