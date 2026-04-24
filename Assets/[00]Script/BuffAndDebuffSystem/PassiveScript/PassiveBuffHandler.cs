using System.Collections.Generic;
using UnityEngine;
using static EffectEnums;
using static ManagerSound;

public class PassiveBuffHandler : MonoBehaviour
{
    [Tooltip("Drag PassiveEffect ScriptableObject assets here - one per rule")]
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
                OnRuleActivated(rules[i]);
            }
            else if (!shouldBeActive && isActive)
            {
                _activeRules.Remove(i);
                dirty = true;
                OnRuleDeactivated(rules[i]);
            }
        }

        if (dirty) Recalculate();
    }

    // ── SFX hooks ─────────────────────────────────────────────────────────

    private void OnRuleActivated(PassiveStatModifier rule)
    {
        if (rule.loopWhileActive)
        {
            // Start looping SFX — stops when deactivated
            if (!string.IsNullOrEmpty(rule.sfxOnActivate))
                LoopEffect(rule.sfxOnActivate);
        }
        else
        {
            // One-shot on activate
            if (!string.IsNullOrEmpty(rule.sfxOnActivate))
                PlayEffect(rule.sfxOnActivate);
        }
    }

    private void OnRuleDeactivated(PassiveStatModifier rule)
    {
        // Stop loop if it was looping
        if (rule.loopWhileActive && !string.IsNullOrEmpty(rule.sfxOnActivate))
            StopLoopEffect(rule.sfxOnActivate);

        // One-shot on deactivate
        if (!string.IsNullOrEmpty(rule.sfxOnDeactivate))
            PlayEffect(rule.sfxOnDeactivate);
    }

    // ── Condition evaluation ──────────────────────────────────────────────

    private bool EvaluateRule(PassiveStatModifier rule)
    {
        float current = 0f;

        switch (rule.watchStat)
        {
            case PassiveWatchStat.HeatGauge:
                float maxGauge = Mathf.Max(_stats.maxHeatGauge, 1f);
                current = _hotGauge != null ? _hotGauge.CurrentGauge / maxGauge : 0f;
                break;

            case PassiveWatchStat.Speed:
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

    // ── Stat recalculation ────────────────────────────────────────────────

    private void Recalculate()
    {
        var bundle = new StatModifierBundle();

        foreach (int i in _activeRules)
            bundle.AddPassive(rules[i]);

        _stats.ApplyPassiveModifiers(bundle);
    }
}