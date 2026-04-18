using System;
using UnityEngine;
using UnityEngine.Events;

public class HotGauge : MonoBehaviour
{
    [Header("SettingHetGauge")]
    private float MaxGauge = 100f;
    private float rateIncrease = 5f;
    private float rateDecrease = 3f;
    private float _CurrentGauge = 0;

    public UnityEvent OnGameOver;
    private bool _isGameOver = false;
    public float CurrentGauge
    {
        get => _CurrentGauge;
        set => _CurrentGauge = value;
    }

    public bool _isIncover;

    private CharacterStats m_characterStats;

    private void Awake()
    {
        

    }

    private void RelinkStats() {
        rateIncrease = m_characterStats.currentHeatIncreaseRate;
        rateDecrease = m_characterStats.baseHeatDecreaseRate;
        MaxGauge = m_characterStats.maxHeatGauge;
    }

    private void Start()
    {
        _CurrentGauge = 0f;

        m_characterStats = GetComponent<CharacterStats>();

        //connect to Character Stats for buff and debuff
        rateIncrease = m_characterStats.currentHeatIncreaseRate + m_characterStats.baseHeatIncreaseRate;
        rateDecrease = m_characterStats.baseHeatDecreaseRate;
        MaxGauge = m_characterStats.maxHeatGauge;
    }
    private void Update()
    {
        if (_isGameOver) return ;
        GameOver();
        RelinkStats(); // get stats from Character stats
        UpdateHetGauge();
        Debug.Log(_CurrentGauge);
    }
    private void GameOver()
    {
        if(_CurrentGauge >= MaxGauge)
        {
            _isGameOver = true ;
            OnGameOver?.Invoke();
        }
    }
    private void UpdateHetGauge()
    {
        if(!_isIncover)
        {
            _CurrentGauge += Time.deltaTime * rateIncrease;
        }
        else
        {
            _CurrentGauge -= Time.deltaTime * rateDecrease;
        }

        if(_CurrentGauge <= 0)
        {
            _CurrentGauge = 0;
        }
    }
}
