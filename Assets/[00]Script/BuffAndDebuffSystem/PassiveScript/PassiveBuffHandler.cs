using System.Collections.Generic;
using UnityEngine;
using static EffectEnums;

public class PassiveBuffHandler : MonoBehaviour
{
    [Tooltip("Drag PassiveEffect ScriptableObject assets here — one per rule")]
    public List<PassiveStatModifier> rules = new();

    private CharacterStats _stats;
    private HotGauge _hotGauge;

    // Track which rules are currently active so we only Recalculate on a state change
    private readonly HashSet<int> _activeRules = new();

    void Awake()
    {
        _stats = GetComponent<CharacterStats>();
        _hotGauge = GetComponent<HotGauge>();
    }

    void Update()
    {
        bool dirty = false;

        for (int i = 0; i < rules.Count; i++)
        {
            if (rules[i] == null) continue;

            bool shouldBeActive = EvaluateRule(rules[i]);
            bool isActive = _activeRules.Contains(i);

            if (shouldBeActive && !isActive)
            {
                _activeRules.Add(i);
                dirty = true;
            }
            else if (!shouldBeActive && isActive)
            {
                _activeRules.Remove(i);
                dirty = true;
            }
        }

        if (dirty) Recalculate();
    }

    private bool EvaluateRule(PassiveStatModifier rule)
    {
        // Get the raw current value for the watched stat
        float current = 0f;

        switch (rule.watchStat)
        {
            case PassiveWatchStat.HeatGauge:
                // HeatGauge uses a 0-1 percentage so thresholdValue is comparable (e.g. 0.70 = 70%)
                float maxGauge = Mathf.Max(_stats.maxHeatGauge, 1f);
                current = _hotGauge != null ? _hotGauge.CurrentGauge / maxGauge : 0f;
                break;

            case PassiveWatchStat.Speed:
                // Speed uses the raw flat unit value (e.g. thresholdValue = 3 means 3 units/s)
                current = _stats.currentMaxSpeed;
                break;
        }

        float lo = rule.thresholdValue;
        float hi = rule.thresholdValueMax;

        return rule.mode switch
        {
            ThresholdMode.AboveOrEqual => current >= lo,
            ThresholdMode.Above => current > lo,
            ThresholdMode.BelowOrEqual => current <= lo,
            ThresholdMode.Below => current < lo,
            ThresholdMode.Between => current >= lo && current <= hi,
            _ => false
        };
    }

    // Sum all active passive modifiers and push into CharacterStats
    private void Recalculate()
    {
        var bundle = new StatModifierBundle();

        foreach (int i in _activeRules)
            bundle.AddPassive(rules[i]);

        _stats.ApplyPassiveModifiers(bundle);
    }
}