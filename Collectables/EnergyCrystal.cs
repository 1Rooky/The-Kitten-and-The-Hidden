using UnityEngine;
using UnityEngine.Events;

public class EnergyCrystal : Crystal
{
    [SerializeField] private float _energyAmount;

    internal static UnityEvent<float> _onEnergyCrystalCollected = new();

    public float EnergyAmount => _energyAmount;

    protected override void Collect(ObjectType _playerType)
    {
        base.Collect(_playerType);
        _onEnergyCrystalCollected?.Invoke(_energyAmount);
    }
}