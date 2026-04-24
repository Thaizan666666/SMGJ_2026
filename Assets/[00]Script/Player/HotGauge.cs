using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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
    public Image BGGauge;
    public List<Sprite> SpritesVisual;
    public bool IsInCover;

    private SetUpMAT stm;
    private CharacterStats _stats;

    public float CurrentGauge => _currentGauge;

    private void Start()
    {
        _stats = GetComponent<CharacterStats>();
        _currentGauge = 0f;
        RelinkStats();
        UpdateHeatGauge();
        UpdateHeatUI();

        stm = FindAnyObjectByType<SetUpMAT>();
        if (stm == null)
            Debug.LogWarning("can't found stm");
    }

    private void Update()
    {
        if (_isGameOver) return;

        RelinkStats();
        UpdateHeatGauge();
        CheckGameOver();
    }

    private void RelinkStats()
    {
        _rateIncrease = _stats.currentHeatIncreaseRate;
        _rateDecrease = _stats.baseHeatDecreaseRate;
        _maxGauge = Mathf.Max(_stats.maxHeatGauge, 1f);
    }

    private void UpdateHeatGauge()
    {
        if (!IsInCover)
            _currentGauge += _rateIncrease * Time.deltaTime;
        else
            _currentGauge -= _rateDecrease * Time.deltaTime;

        _currentGauge = Mathf.Clamp(_currentGauge, minHeatGauge, _maxGauge);

        _stats.currentHotGauge = _currentGauge;
        UpdateHeatUI();
    }

    public void AddToGauge(float delta)
    {
        _currentGauge = Mathf.Clamp(_currentGauge + delta, minHeatGauge, _maxGauge);
        _stats.currentHotGauge = _currentGauge;
    }

    private void CheckGameOver()
    {
        if (_currentGauge < _maxGauge) return;

        stm.ClearScreen();
        _isGameOver = true;
        StopAllLoopEffect();
        PlayEffect("BadEnding");
        ManagerScene.Instance.LoadSunBurnEnding();
    }

    private void UpdateHeatUI()
    {
        if (HeatGauge != null)
            HeatGauge.value = _currentGauge / _maxGauge;

        if (BGGauge == null || SpritesVisual == null || SpritesVisual.Count < 4) return;

        if (_currentGauge >= 75f)
            BGGauge.sprite = SpritesVisual[3];
        else if (_currentGauge >= 50f)
            BGGauge.sprite = SpritesVisual[2];
        else if (_currentGauge >= 25f)
            BGGauge.sprite = SpritesVisual[1];
        else
            BGGauge.sprite = SpritesVisual[0];
    }
}