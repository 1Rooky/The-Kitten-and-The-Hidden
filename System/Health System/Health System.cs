using UnityEngine;
using UnityEngine.Events;

// This class represents a health system for a game object.
public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _healthMax; // The maximum health of the game object.

    // UnityEvents that are triggered when the health changes, the game object dies, heals, or takes damage.
    [SerializeField] private UnityEvent _onHealthChanged;

    [SerializeField] private UnityEvent _onHeal;
    [SerializeField] private UnityEvent _onDamageTaken;
    [SerializeField] protected UnityEvent _onDeath;

    protected float _health; // The current health of the game object.

    // Property that calculates the current health percentage.
    internal float HealthPrecentage => (_health / _healthMax);

    // Property that returns the OnHealthChanged event.
    internal UnityEvent OnHealthChanged => _onHealthChanged;

    // Initialize the health to the maximum health when the game object awakes.
    private void Awake() => _health = _healthMax;

    // Method to reduce the health of the game object.
    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount; // Reduce the health by the damage amount.
        // If the health drops to 0 or below, invoke the _onDeath event.
        if (_health <= 0)
        {
            _health = 0;
            _onDeath?.Invoke();
        }

        // Invoke the _onHealthChanged and _onDamageTaken events.
        _onHealthChanged?.Invoke();
        _onDamageTaken?.Invoke();
    }

    // Method to increase the health of the game object.
    public void Heal(float healAmount)
    {
        _health += healAmount; // Increase the health by the heal amount.

        // If the health exceeds the maximum health, set it to the maximum health.
        if (_health >= _healthMax)
            _health = _healthMax;

        // Invoke the _onHealthChanged and _onHeal events.
        _onHealthChanged?.Invoke();
        _onHeal?.Invoke();
    }

    // Method to set the health to 0 and invoke the _onDeath event.
    public void Die()
    {
        _health = 0; // Set the health to 0.
        _onDeath?.Invoke(); // Invoke the _onDeath event.
    }
    // Method to get the Transform component of the game object.
    public Transform GetTransform() => transform;
}