using System;
using UnityEngine;
using UnityEngine.Events;

public class HotGauge : MonoBehaviour
{
    [Header("SettingHetGauge")]
    [SerializeField] private float MaxGauge = 100f;
    [SerializeField] private float rateIncrease = 5f;
    [SerializeField] private float rateDeincrease = 3f;
    private float _CurrentGauge = 0;

    public UnityEvent OnGameOver;
    private bool _isGameOver = false;
    public float CurrentGauge => _CurrentGauge;
    public bool _isIncover;

    private void Start()
    {
        _CurrentGauge = 0f;
    }
    private void Update()
    {
        if (_isGameOver) return ;
        GameOver();
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
            _CurrentGauge -= Time.deltaTime * rateDeincrease;
        }

        if(_CurrentGauge <= 0)
        {
            _CurrentGauge = 0;
        }
    }
}
