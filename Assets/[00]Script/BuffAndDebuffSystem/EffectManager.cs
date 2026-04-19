using System.Collections.Generic;
using UnityEngine;
using static EffectEnums;

[RequireComponent(typeof(CharacterStats))]
public class EffectManager : MonoBehaviour
{
    private CharacterStats _stats;
    private List<ActiveTimedEffect> _timedEffects = new();

    void Awake()
    {
        _stats = GetComponent<CharacterStats>();
    }

    // ── Timed ─────────────────────────────────────────────────────────────

    public void ApplyTimed(TimedEffectData data)
    {
        if (data == null)
        {
            Debug.LogWarning("ApplyTimed: TimedEffectData is null — did you forget to assign the ScriptableObject?", this);
            return;
        }

        // Refresh duration if already active — value does not stack
        var existing = _timedEffects.Find(e => e != null && e.Data != null && e.Data.effectId == data.effectId);
        if (existing != null)
        {
            existing.RemainingTime = data.duration;
            return;
        }

        _timedEffects.Add(new ActiveTimedEffect(data));
        Recalculate();
    }

    public void RemoveTimedById(string effectId)
    {
        int removed = _timedEffects.RemoveAll(e => e.Data.effectId == effectId);
        if (removed > 0) Recalculate();
    }

    void Update()
    {
        bool dirty = false;

        for (int i = _timedEffects.Count - 1; i >= 0; i--)
        {
            // Remove any corrupted entries silently
            if (_timedEffects[i] == null || _timedEffects[i].Data == null)
            {
                _timedEffects.RemoveAt(i);
                dirty = true;
                continue;
            }

            _timedEffects[i].RemainingTime -= Time.deltaTime;
            if (_timedEffects[i].RemainingTime <= 0f)
            {
                _timedEffects.RemoveAt(i);
                dirty = true;
            }
        }

        if (dirty) Recalculate();
    }

    // Sum all active timed modifiers across all their stat entries and push into CharacterStats
    private void Recalculate()
    {
        var bundle = new StatModifierBundle();

        foreach (var effect in _timedEffects)
            foreach (var entry in effect.Data.statEntries)
                bundle.Add(entry);

        _stats.RecalculateTimed(bundle);
    }

    // ── Instant (one-shot, no undo) ───────────────────────────────────────

    public void ApplyInstant(InstantEffectData data)
    {
        if (data == null)
        {
            Debug.LogWarning("ApplyInstant: InstantEffectData is null — did you forget to assign the ScriptableObject?", this);
            return;
        }

        float sign = data.mode == ModifierMode.Add ? 1f : -1f;
        float signed = data.flatValue * sign;

        switch (data.stat)
        {
            case InstantStatType.Speed: _stats.ApplyInstantSpeed(signed); break;
            case InstantStatType.HeatGauge: _stats.ApplyInstantHeatGauge(signed); break;
        }
    }
}