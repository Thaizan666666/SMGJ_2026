using UnityEngine;
using UnityEngine.Events;

public class HotGauge : MonoBehaviour
{
    [Header("Heat Gauge Settings")]
    private float _maxGauge = 100f;
    private float _rateIncrease = 5f;
    private float _rateDecrease = 3f;

    private float _currentGauge = 0f;
    private bool _isGameOver = false;

    public UnityEvent OnGameOver;

    public bool _isIncover;

    // Read-only property — other systems read this, only HotGauge writes it
    public float CurrentGauge => _currentGauge;

    // CharacterStats drives all rates via RelinkStats
    private CharacterStats _stats;

    private void Start()
    {
        _stats = GetComponent<CharacterStats>();
        _currentGauge = 0f;
        RelinkStats();
    }

    private void Update()
    {
        if (_isGameOver) return;

        RelinkStats();
        UpdateHeatGauge();
        CheckGameOver();

        //Debug.Log($"Current Heat Gauge: {_currentGauge:F1}");
    }

    // Pull current (post-modifier) rates from CharacterStats every frame
    private void RelinkStats()
    {
        _rateIncrease = _stats.currentHeatIncreaseRate;  // already has timed modifiers baked in
        _rateDecrease = _stats.baseHeatDecreaseRate;
        _maxGauge = _stats.maxHeatGauge;
    }

    private void UpdateHeatGauge()
    {
        if (!_isIncover)
            _currentGauge += _rateIncrease * Time.deltaTime;
        else
            _currentGauge -= _rateDecrease * Time.deltaTime;

        _currentGauge = Mathf.Clamp(_currentGauge, 0f, _maxGauge);

        // Keep CharacterStats mirror in sync so EffectManager instant writes are visible
        _stats.currentHotGauge = _currentGauge;
    }

    private void CheckGameOver()
    {
        if (_currentGauge >= _maxGauge)
        {
            _isGameOver = true;
            OnGameOver?.Invoke();
        }
    }

    // Called by EffectManager.ApplyInstant for HeatGauge instant effects
    public void AddToGauge(float delta)
    {
        _currentGauge = Mathf.Clamp(_currentGauge + delta, 0f, _maxGauge);
        _stats.currentHotGauge = _currentGauge;
    }
}