using System.Collections;
using UnityEngine;

public class GhostEnergySystem : EnergySystem
{
    private Coroutine _energyDecreaseCoroutine;

    private void OnEnable()
    {
        EnergyCrystal._onEnergyCrystalCollected.AddListener(GainEnergy);
        _onNoEnergy.AddListener(SpawnToLastCheckPoint);
    }

    private void OnDisable()
    {
        EnergyCrystal._onEnergyCrystalCollected.RemoveListener(GainEnergy);
        _onNoEnergy.RemoveListener(SpawnToLastCheckPoint);
    }

    private void SpawnToLastCheckPoint()
    {
        ServiceLocator.Instance.GetService<SpawnSystem>().SpawnAtLastCheckPoint();
        _energy = 0.5f * _maxEnergy;
    }

    public void StartEnergyDecrease(float _energyDecreaseRate) => _energyDecreaseCoroutine ??= StartCoroutine(DecreaseEnergyOverTime(_energyDecreaseRate));

    public void StopEnergyDecrease()
    {
        if (_energyDecreaseCoroutine != null)
        {
            StopCoroutine(_energyDecreaseCoroutine);
            _energyDecreaseCoroutine = null;
        }
    }

    private IEnumerator DecreaseEnergyOverTime(float _energyDecreaseRate)
    {
        while (_energy > 0)
        {
            _energy -= _energyDecreaseRate * Time.deltaTime;

            if (_energy <= 0)
            {
                _energy = 0;
                _onNoEnergy?.Invoke();
            }
            Logging.Log(_energy);
            OnEnergyChanged?.Invoke();
            yield return null;
        }
    }
}