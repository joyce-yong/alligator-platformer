using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerJetpack : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float jetpackForce = 3f;
    [SerializeField] private float jetpackFuel = 5f;

    private float _fuelLeft;
    private float _fuelDurectionLeft;
    private bool _stillHaveFuel = true;

    private int _jetpackParameter = Animator.StringToHash("Jetpack");

    protected override void InitState()
    {
        base.InitState();
        _fuelDurectionLeft = jetpackFuel;
        _fuelLeft = jetpackFuel;
        UIManager.Instance.UpdateFuel(_fuelLeft, jetpackFuel);
    }

    protected override void GetInput()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Jetpack();
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            EndJetpack();
        }
    }

    private void Jetpack()
    {
        if (!_stillHaveFuel)
        {
            return;
        }

        if (_fuelLeft <= 0)
        {
            EndJetpack();
            _stillHaveFuel = false;
            return;
        }

        _playerController.SetVerticalForce(jetpackForce);
        _playerController.Conditions.IsJetpacking = true;
        StartCoroutine(BurnFuel());
    }

    private void EndJetpack()
    {
        _playerController.Conditions.IsJetpacking = false;
        StartCoroutine(Refill());
    }

    private IEnumerator BurnFuel()
    {
        float fuelConsumed = _fuelLeft;
        if (fuelConsumed > 0 && _playerController.Conditions.IsJetpacking && _fuelLeft <= fuelConsumed)
        {
            fuelConsumed -= Time.deltaTime;
            _fuelLeft = fuelConsumed;
            UIManager.Instance.UpdateFuel(_fuelLeft, jetpackFuel);
            yield return null;
        }
    }

    private IEnumerator Refill()
    {
        yield return new WaitForSeconds(0.5f);
        float fuel = _fuelLeft;
        while (fuel < jetpackFuel && !_playerController.Conditions.IsJetpacking)
        {
            fuel += Time.deltaTime;
            _fuelLeft = fuel;
            UIManager.Instance.UpdateFuel(_fuelLeft, jetpackFuel);

            if (!_stillHaveFuel && fuel > 0.2f)
            {
                _stillHaveFuel = true;
            }

            yield return null;
        }
    }

    public override void SetAnimation()
    {
        _animator.SetBool(_jetpackParameter, _playerController.Conditions.IsJetpacking);
    }
}
