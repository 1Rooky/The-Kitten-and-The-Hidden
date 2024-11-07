using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

// This component requires a SphereCollider to function properly. It's used to define the attack radius of an entity.
[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleEffect; // Particle effect to play on attack
    [SerializeField] private SphereCollider _sphereCollider; // The collider that defines the attack radius

    private int _damage = 10; // The amount of damage dealt by the attack
    private float _attackDelay = 0.5f; // The delay between attacks to prevent spamming

    protected readonly List<IDamageable> _damageables = new(); // List of objects within the attack radius that can be damaged
    protected Coroutine _attackCoroutine; // Coroutine for handling the attack logic
    protected Transform _transform; // Cached transform component for performance
    private WaitForSeconds _wait; // WaitForSeconds object used for attack delays, to avoid creating new ones each time
    private ObjectPool _objectPool; // Reference to an object pool for reusing particle effects

    // Event and delegate for handling attack actions
    public event AtackEvent OnAttack;

    public delegate void AtackEvent(IDamageable _target);

    private IDamageable _closestDamageable; // The closest damageable object to the attacker

    // Properties for accessing and modifying private fields
    internal float SphereColliderAttackRadius { get => _sphereCollider.radius; set => _sphereCollider.radius = value; }

    internal int Damage { get => _damage; set => _damage = value; }
    internal float AttackDelay { get => _attackDelay; set => _attackDelay = value; }
    internal ObjectPool ObjectPool { get => _objectPool; set => _objectPool = value; }
    internal ParticleSystem ParticleEffect { get => _particleEffect; }

    protected virtual void Start()
    {
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>(); // Initialize the object pool
        _wait = new WaitForSeconds(_attackDelay); // Initialize the WaitForSeconds object
        _transform = transform; // Cache the transform component
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // Add damageable objects that enter the attack radius to the list and start the attack coroutine if not already running
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageables.Add(damageable);
            _attackCoroutine ??= StartCoroutine(Attack());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // Remove damageable objects that exit the attack radius from the list and stop the attack coroutine if the list is empty
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            _damageables.Remove(damageable);
            if (_damageables.Count == 0)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }

    // Coroutine that handles the attack logic, including finding the closest target and applying damage
    protected virtual System.Collections.IEnumerator Attack()
    {
        yield return _wait;

        _closestDamageable = null;
        float _closestDistance = float.MaxValue;

        while (_damageables.Count > 0)
        {
            foreach (var damageable in _damageables)
            {
                var distance = Vector3.Distance(_transform.position, damageable.GetTransform().position);
                if (distance < _closestDistance)
                {
                    _closestDistance = distance;
                    _closestDamageable = damageable;
                }
            }

            if (_closestDamageable != null)
                InvokeOnAttack(_closestDamageable);

            _closestDistance = float.MaxValue;

            yield return _wait;

            _damageables.RemoveAll(DisableDamageable); // Clean up any damageable objects that are no longer active
        }

        _attackCoroutine = null;
    }

    // Helper method to determine if a damageable object is disabled
    protected bool DisableDamageable(IDamageable damageable) => damageable != null && !damageable.GetTransform().gameObject.activeSelf;

    // Method to invoke the OnAttack event
    protected void InvokeOnAttack(IDamageable target) => OnAttack?.Invoke(target);

    // Method called when the attack animation is completed to apply damage and play particle effects
    internal virtual void OnAttackAnimationCompleted()
    {
        if (_closestDamageable != null)
        {
            _closestDamageable.TakeDamage(Damage);

            if (_particleEffect != null)
            {
                GameObject _particleEffectGameObject = _objectPool.GetPooledObject(_particleEffect.gameObject);
                _particleEffectGameObject.transform.position = _closestDamageable.GetTransform().position;
                ParticleSystem particleInstance = _particleEffectGameObject.GetComponent<ParticleSystem>();
                particleInstance.Play();

                DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
            }

            _closestDamageable = null;
        }
    }
}