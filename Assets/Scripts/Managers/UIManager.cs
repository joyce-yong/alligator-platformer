using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Settings")]
    [SerializeField] private Image fuelImage;

    private float _currentJetpackFuel;
    private float _jetpackFuel;

    private void Update()
    {
        InternalJetpackUpdate();
    }

    // Gets the fuel values
    public void UpdateFuel(float currentFuel, float maxFuel)
    {
        _currentJetpackFuel = currentFuel;
        _jetpackFuel = maxFuel;
    }

    // Updates the jetpack fuel
    private void InternalJetpackUpdate()
    {
        fuelImage.fillAmount =
            Mathf.Lerp(fuelImage.fillAmount, _currentJetpackFuel / _jetpackFuel, Time.deltaTime * 10f);
    }
}
