using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

// Inherits from AttackRadius to provide functionality for ranged attacks, including homing bullets and line of sight checks
public class RangedAttackRadius : AttackRadius
{
    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent _navMeshAgent; // Reference to the NavMeshAgent for movement control

    [SerializeField] private float _sphereCastRadius = 0.1f; // Radius for the SphereCast used in line of sight checks

    private bool _useHomingBullet; // Determines if bullets should home in on targets

    private Vector3 _bulletSpawnOffset; // Offset from the attacker's position where bullets are spawned
    private LayerMask _layerMask; // LayerMask to determine what the attack can hit or be blocked by

    private WaitForSeconds _attackDelayWaitForSeconds; // Cached WaitForSeconds to optimize performance in coroutine
    private RaycastHit _hit; // Stores the result of the SphereCast used for line of sight checks

    private IDamageable _targetDamageable; // The current target being attacked
    private IDamageable _storedTargetDamageable; // Stores the target for use after the attack animation completes
    private Bullet _bullet; // The bullet prefab used for ranged attacks

    #region Setters

    // Properties for setting private fields from other scripts
    internal bool UseHomingBullet
    { get => _useHomingBullet; set { _useHomingBullet = value; } }

    internal Bullet Bullet
    {
        set { _bullet = value; }
    }

    internal Vector3 BulletSpawnOffset
    {
        set { _bulletSpawnOffset = value; }
    }

    internal LayerMask LineOfSightLayers
    {
        set { _layerMask = value; }
    }

    #endregion Setters

    protected override void Start()
    {
        base.Start();

        // Initialize required components and variables
        ObjectPool = ServiceLocator.Instance.GetService<ObjectPool>();
        _attackDelayWaitForSeconds = new WaitForSeconds(AttackDelay);
    }

    // Coroutine that handles the attack logic, including delay and line of sight checks
    protected override System.Collections.IEnumerator Attack()
    {
        yield return _attackDelayWaitForSeconds;

        while (_damageables.Count > 0)
        {
            _targetDamageable = null;

            // Check for line of sight to each damageable target
            for (int i = 0; i < _damageables.Count; i++)
            {
                if (HasLineOfSightTo(_damageables[i].GetTransform()))
                {
                    _targetDamageable = _damageables[i];
                    _storedTargetDamageable = _targetDamageable; // Store the target for later use
                    InvokeOnAttack(_targetDamageable);
                    _navMeshAgent.enabled = true;
                    break;
                }
            }

            yield return _attackDelayWaitForSeconds;

            // Re-enable the NavMeshAgent if no target is found or if line of sight is lost
            if (_targetDamageable == null || !HasLineOfSightTo(_targetDamageable.GetTransform()))
                _navMeshAgent.enabled = true;

            // Remove any targets that are no longer valid
            _damageables.RemoveAll(DisableDamageable);
        }

        _navMeshAgent.enabled = true;
        _attackCoroutine = null;
    }

    // Called when the attack animation is completed to spawn and configure the bullet
    internal override void OnAttackAnimationCompleted()
    {
        if (_storedTargetDamageable != null && _bullet != null)
        {
            if (ObjectPool == null)
            {
                Logging.LogError("ObjectPool is null. Please assign the ObjectPool in the inspector.");
                return;
            }

            // Get a bullet from the object pool and configure it based on the attack settings
            if (_useHomingBullet)
                _bullet = ObjectPool.GetPooledObject(_bullet.gameObject).GetComponent<HomingBullet>();
            else
                _bullet = ObjectPool.GetPooledObject(_bullet.gameObject).GetComponent<Bullet>();

            _bullet.Damage = Damage;
            _bullet.transform.SetPositionAndRotation(transform.position + _bulletSpawnOffset, _navMeshAgent.transform.rotation);

            // Spawn the bullet towards the stored target
            _bullet.Spawn(_navMeshAgent.transform.forward, Damage, _storedTargetDamageable.GetTransform());

            // Subscribe to the bullet's OnHit event to trigger effects upon hitting the target
            _bullet.OnHit += Bullet_OnHit;

            _storedTargetDamageable = null; // Reset the stored target after shooting
        }
    }

    // Handles the logic for when a bullet hits a target, including spawning particle effects
    private void Bullet_OnHit()
    {
        if (ParticleEffect != null)
        {
            GameObject _particleEffectGameObject = ObjectPool.GetPooledObject(ParticleEffect.gameObject);
            if (_particleEffectGameObject != null)
            {
                _particleEffectGameObject.transform.position = _bullet.transform.position; // Position the particle effect
                ParticleSystem particleInstance = _particleEffectGameObject.GetComponent<ParticleSystem>();
                particleInstance.Play();

                // Use DOVirtual.DelayedCall to deactivate the particle effect GameObject after it finishes playing
                float totalDuration = particleInstance.main.duration + particleInstance.main.startLifetime.constantMax;
                DOVirtual.DelayedCall(totalDuration, () =>
                {
                    _particleEffectGameObject.SetActive(false);
                });

                _bullet.OnHit -= Bullet_OnHit; // Unsubscribe to prevent memory leaks
            }
        }
    }

    // Checks if there is a clear line of sight to the target using a SphereCast
    private bool HasLineOfSightTo(Transform _target)
    {
        Vector3 origin = _transform.position + _bulletSpawnOffset;
        Vector3 direction = (_target.position + _bulletSpawnOffset - origin).normalized;

        if (Physics.SphereCast(origin, _sphereCastRadius, direction, out _hit, SphereColliderAttackRadius * 2, _layerMask))
            if (_hit.transform.TryGetComponent(out IDamageable damageable))
                return damageable.GetTransform() == _target;

        return false;
    }

    // Ensures the NavMeshAgent is re-enabled when the target exits the attack radius
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (_attackCoroutine == null)
            _navMeshAgent.enabled = true;
    }
}