using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ManagerSound;

public class HotGauge : MonoBehaviour
{
    [Header("Heat Gauge Settings")]
    private float _maxGauge = 100f;
    private float _rateIncrease = 5f;
    private float _rateDecrease = 3f;

    [Tooltip("Gauge can never drop below this value (floor)")]
    public float minHeatGauge = 0f;

    private float _currentGauge = 0f;
    private bool _isGameOver = false;
    public bool _IsGameOver => _isGameOver;

    [Header("Gauge Visualized")]
    public Slider HeatGauge;

    public bool IsInCover;

    // Read-only — other systems read this value, only HotGauge writes it
    public float CurrentGauge => _currentGauge;

    private CharacterStats _stats;

    private void Start()
    {
        _stats = GetComponent<CharacterStats>();
        _currentGauge = 0f;
        UpdateHeatGauge();
        UpdateHeatUI();
        RelinkStats();
    }

    private void Update()
    {
        if (_isGameOver) return;

        
        RelinkStats();
        CheckGameOver();

        //Debug.Log($"Current Heat Gauge: {_currentGauge:F1} / {_maxGauge:F0}");
        UpdateHeatGauge();
    }

    // Pull current (post-modifier) rates from CharacterStats every frame
    private void RelinkStats()
    {
        _rateIncrease = _stats.currentHeatIncreaseRate;
        _rateDecrease = _stats.baseHeatDecreaseRate;
        // Guard: maxHeatGauge must never be zero or negative
        _maxGauge = Mathf.Max(_stats.maxHeatGauge, 1f);
    }

    private void UpdateHeatGauge()
    {
        if (!IsInCover)
            _currentGauge += _rateIncrease * Time.deltaTime;
        else
            _currentGauge -= _rateDecrease * Time.deltaTime;

        _currentGauge = Mathf.Clamp(_currentGauge, minHeatGauge, _maxGauge);

        UpdateHeatUI(); 

        // Mirror into CharacterStats
        _stats.currentHotGauge = _currentGauge;
    }

    private void CheckGameOver()
    {
        if (_currentGauge >= _maxGauge)
        {
            _isGameOver = true;
            StopAllLoopEffect();
            PlayEffect("Sunburn");

            ManagerScene.Instance.LoadSunBurnEnding();
            

        }
    }

    // Called by CharacterStats.ApplyInstantHeatGauge for instant effects
    public void AddToGauge(float delta)
    {
        _currentGauge = Mathf.Clamp(_currentGauge + delta, minHeatGauge, _maxGauge);
        _stats.currentHotGauge = _currentGauge;
    }

    private void UpdateHeatUI()
    {
        if (HeatGauge != null)
            HeatGauge.value = _currentGauge / _maxGauge;
    }
}