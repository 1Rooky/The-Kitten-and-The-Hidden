using UnityEngine;
using UnityEngine.Events;

public class EnergySystem : MonoBehaviour
{
    [SerializeField] protected float _maxEnergy;
    [SerializeField] protected float _startEnergy;

    [SerializeField] protected UnityEvent _onEnergyChanged;
    [SerializeField] protected UnityEvent _onMaxEnergy;
    [SerializeField] protected UnityEvent _onNoEnergy;
    [SerializeField] protected UnityEvent _onGainEnergy;
    [SerializeField] protected UnityEvent _onLoseEnergy;

    // [SerializeField] protected float _minEnergyRange;
    //[SerializeField] protected UnityEvent _onLowEnergy;  //if needed.

    protected float _energy = 0;
    internal float EnergyPrecentage => (_energy / _maxEnergy);
    internal UnityEvent OnEnergyChanged => _onEnergyChanged;

    private void Start() => _energy = _startEnergy;

    public void GainEnergy(float energyAmount)
    {
        _energy += energyAmount;

        if (_energy > _maxEnergy)
        {
            _energy = _maxEnergy;
            _onMaxEnergy?.Invoke();
        }

        OnEnergyChanged?.Invoke();
        _onGainEnergy?.Invoke();
    }

    public void LoseEnergy(float energyAmount)
    {
        _energy -= energyAmount;

        if (_energy <= 0)
        {
            _energy = 0;
            _onNoEnergy?.Invoke();
        }

        // To apply effect on low energy can be used later? Note: Still needs alittle edit to be used.

        //if (_energy <= _minEnergyRange)
        //{
        //    OnLowEnergy?.Invoke();
        //}

        OnEnergyChanged?.Invoke();
        _onLoseEnergy?.Invoke();
    }
}