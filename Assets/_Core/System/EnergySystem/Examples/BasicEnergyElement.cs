using System;
using TMPro;
using UnityEngine;

public class BasicEnergyElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _energyText;
    [SerializeField] private TMP_Text _energyTimerText;
    private SystemLocator _systemLocator;
    private readonly string _energyTimerFormat = "{0:D2}:{1:D2}";
    private int maxEnergy;
    private void Awake()
    {
#if EnergyManager_Enabled
        maxEnergy = GameInstaller.Instance.SystemLocator.RemoteConfigManager.GetObject<EnergyConfig>().MaxEnergy;
#endif
        _systemLocator = GameInstaller.Instance.SystemLocator;
    }
    private void OnEnable()
    {
        if (_energyText)
        {
            _energyText.text = _systemLocator.EnergyManager.Get().ToString();
        }
        _systemLocator.EnergyManager.OnEnergyChanged += OnEnergyChanged;
    }
    private float _timer;
    private float _remainingTime;
    private TimeSpan _timeSpan;
    private void Update()
    {
        if (_timer>Time.time-0.25f)return;
        if (_systemLocator.EnergyManager.GetUnlimitedEnergyTime().Item1)
        {
            if(_energyText) _energyText.text = "∞";
        }else if (_systemLocator.EnergyManager.Get() >= maxEnergy)
        {
            if(_energyText) _energyText.text = _systemLocator.EnergyManager.Get().ToString();
            if(_energyTimerText) _energyTimerText.text = "FULL";
            return;
        }
        else if (_energyText!=null)
            if(_energyText) _energyText.text = _systemLocator.EnergyManager.Get().ToString();
           
        _remainingTime = _systemLocator.EnergyManager.GetRemainingTime();
        _timeSpan = TimeSpan.FromSeconds(_remainingTime);
        if (_timeSpan.Hours>1)
        {
            if(_energyTimerText)_energyTimerText.text = _remainingTime >= 0 ? string.Format(_energyTimerFormat,_timeSpan.Hours,_timeSpan.Minutes):"";
        }else
            _energyTimerText.text = _remainingTime >= 0 ? string.Format(_energyTimerFormat,_timeSpan.Minutes,_timeSpan.Seconds):"";
        _timer = Time.time;
    }


    private void OnDisable()
    {
        _systemLocator.EnergyManager.OnEnergyChanged -= OnEnergyChanged;
    }

    private void OnEnergyChanged(int obj)
    {
        if (_energyText)
        {
            _energyText.text = obj.ToString();
        }
    }

    public void AddEnergy()
    {
        _systemLocator.EnergyManager.Add(1);
    }
}