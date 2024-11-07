using UnityEngine;
using UnityEngine.Events;

// This class represents a health crystal in the game, which is a type of crystal that can heal the player.
public class HealthCrystal : Crystal
{
    [SerializeField] private float _healAmount; // The amount of health that the crystal heals.

    // Event that is triggered when a health crystal is collected. The heal amount is passed as a parameter.
    internal static UnityEvent<float> _onHealthCrystalCollected = new();

    // Override the Collect method from the Crystal base class.
    protected override void Collect(ObjectType _objectTypeType)
    {
        base.Collect(_objectTypeType); // Call the base Collect method.

        // Invoke the _onHealthCrystalCollected event and pass the heal amount as a parameter.
        _onHealthCrystalCollected?.Invoke(_healAmount);
    }
}