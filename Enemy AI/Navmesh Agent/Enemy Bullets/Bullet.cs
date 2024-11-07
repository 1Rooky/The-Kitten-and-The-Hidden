using System;
using UnityEngine;

// This component requires a Rigidbody to function properly. It handles bullet behavior including movement, damage application, and auto-destruction.
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _autoDestoryByTime = 5f; // Time after which the bullet will automatically destroy itself
    [SerializeField] private float _speed = 2f; // Speed at which the bullet moves
    [SerializeField] private int _damage = 10; // Damage the bullet will deal upon hitting a damageable target

    public event Action OnHit; // Event triggered when the bullet hits a target

    protected Transform _target; // The target the bullet is aimed at

    private Rigidbody _rigidbody; // Reference to the bullet's Rigidbody component
    private float _timeEnabled; // Time at which the bullet was enabled

    // Properties to get and set bullet attributes
    internal float AutoDestoryByTime { set => _autoDestoryByTime = value; }

    internal float Speed { get => _speed; set => _speed = value; }
    internal int Damage { set => _damage = value; }
    internal Rigidbody Rigidbody { get => _rigidbody; }
    internal (Vector3, ForceMode) RigidbodyAddForce { set => _rigidbody.AddForce(value.Item1, value.Item2); }

    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        // Attempt to get the Rigidbody component and log an error if it fails
        if (!TryGetComponent(out _rigidbody))
            Logging.LogError("Failed to get Rigidbody component");
    }

    // OnEnable is called when the object becomes enabled and active
    protected virtual void OnEnable() => _timeEnabled = Time.time;

    // Update is called once per frame
    protected virtual void Update()
    {
        // Check if the bullet should be automatically destroyed based on the elapsed time
        float currentTime = Time.time;
        if (currentTime - _timeEnabled >= _autoDestoryByTime)
            Disable();
    }

    // OnTriggerEnter is called when the Collider other enters the trigger
    protected virtual void OnTriggerEnter(Collider other)
    {
        // If the bullet hits a damageable object, apply damage and trigger the OnHit event
        if (other.TryGetComponent(out IDamageable damageable))
        {
            OnHit?.Invoke();
            damageable.TakeDamage(_damage);
        }

        // Disable the bullet after it hits something
        Disable();
    }

    // Method to initialize the bullet with direction, damage, and target
    internal virtual void Spawn(Vector3 _forward, int _damage, Transform _target)
    {
        this._damage = _damage;
        this._target = _target;

        // Apply an initial force to the bullet in the specified direction
        _rigidbody.AddForce(_forward * _speed, ForceMode.VelocityChange);
    }

    // Method to disable the bullet and reset its velocity
    protected void Disable()
    {
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}