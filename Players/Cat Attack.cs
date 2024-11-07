using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

// This class handles the attack logic for the cat character, including animation and applying damage.
public class CatAttack : MonoBehaviour
{
    [SerializeField] private int _attackDamage; // Damage dealt by the attack
    [SerializeField] private Animator _animator; // Animator for the cat's animations
    [SerializeField] private float _attackDelay = 1f; // Delay between each attack to prevent spamming
    [SerializeField] private GameObject _hitVFXPrefab; // VFX to play when the attack hits

    private InputManager _inputManager; // Reference to the input manager
    private ObjectPool objectPool; // Reference to the ObjectPool for reusing game objects
    private WaitForSeconds _attackDelayWaitForSeconds; // WaitForSeconds used for attack delay
    private WaitForSeconds _vfxyWaitForSeconds; // WaitForSeconds used for visual effects delay
    private IDamageable _damageable = null; // Reference to the damageable component of the enemy

    private int AttackAnimationId; // Hash ID for the attack animation
    private readonly float animationPlayTransition = 0.001f; // Transition time for starting the attack animation

    private Vector3 _originalScale; // Original scale of the cat, used for animation effects
    private Vector3 _originalPosition; // Original position of the cat, used for animation effects

    private bool _isAttacking = false; // Flag to check if the cat is currently attacking

    private void OnDisable()
    {
        // Unsubscribe to prevent access to destroyed objects
        if (_inputManager != null)
            _inputManager.CatActions.Attack.started -= Attack;
    }

    private void Start()
    {
        // Initialize components and subscribe to input actions
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.CatActions.Attack.started += Attack;

        // Get the ObjectPool from the ServiceLocator for VFX instantiation
        objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Convert the animation name to a hash ID for efficient animation parameter handling
        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Attacking);

        // Cache the original scale and position of the cat
        _originalScale = transform.localScale;
        _originalPosition = transform.position;

        // Initialize the WaitForSeconds objects for delays
        _attackDelayWaitForSeconds = new WaitForSeconds(_attackDelay);
        _vfxyWaitForSeconds = new WaitForSeconds(0.1f);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        // Check if this component is still enabled and not destroyed
        if (this == null || !enabled || _isAttacking) return;

        // Store the current position as the original position
        _originalPosition = transform.position;

        // Start the attack routine
        StartCoroutine(AttackRoutine());
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        // Early exit if the component or GameObject has been destroyed
        if (this == null) yield break;

        _isAttacking = true;
        _animator.CrossFade(AttackAnimationId, animationPlayTransition); // Play attack animation

        // Calculate the distance to the closest enemy
        float distanceToEnemy = _damageable != null ? Vector3.Distance(transform.position, _damageable.GetTransform().position) : float.MaxValue;
        // Ensure there's a minimum distance to the enemy to avoid issues with attacking too close
        float minDistanceForAttack = 0.5f; // Minimum distance to maintain from the enemy before attacking

        // Check if the enemy is extremely close or overlapping
        bool isEnemyOverlapping = distanceToEnemy < Mathf.Epsilon;

        if (distanceToEnemy > minDistanceForAttack || isEnemyOverlapping)
        {
            // If the enemy is extremely close or overlapping, skip moving back
            if (!isEnemyOverlapping)
            {
                // Move slightly back before attacking to ensure the attack registers
                Vector3 stepBackPosition = transform.position - transform.forward * 0.5f;
                transform.DOMove(stepBackPosition, 0.1f).SetEase(Ease.InOutSine);
                yield return _vfxyWaitForSeconds; // Wait for the step back to complete
            }
        }

        // Create a sequence for visual effects during the attack
        Sequence attackSequence = DOTween.Sequence();
        attackSequence.Append(transform.DOScale(_originalScale * 1.1f, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Append(transform.DOMove(transform.position + transform.forward * 1.5f, 0.2f).SetEase(Ease.InOutSine));
        attackSequence.Append(transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutSine));
        attackSequence.Join(transform.DOMove(_originalPosition, 0.2f).SetEase(Ease.InOutSine));

        yield return _attackDelayWaitForSeconds; // Wait for the attack delay

        _isAttacking = false; // Reset the attacking flag
    }

    // This method is called when the attack animation reaches its designated point to apply damage and play VFX
    internal void OnAttackAnimationCompleted()
    {
        // Check if the _damageable is not null and the enemy GameObject is active before applying damage and spawning VFX
        if (_damageable != null && _damageable.GetTransform().gameObject.activeInHierarchy)
        {
            // Rotate the cat to face the enemy instantly
            Vector3 directionToEnemy = _damageable.GetTransform().position - transform.position;
            if (directionToEnemy != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); // Instant rotation with factor 1f
            }

            // Apply damage to the enemy
            _damageable.TakeDamage(_attackDamage);

            // Instantiate and position the hit VFX
            if (_hitVFXPrefab != null)
            {
                GameObject hitVFX = objectPool.GetPooledObject(_hitVFXPrefab);
                Vector3 hitPosition = _damageable.GetTransform().position + (transform.up * 1.5f);
                hitVFX.transform.SetPositionAndRotation(hitPosition, Quaternion.identity);
                hitVFX.SetActive(true);

                // Animate the VFX and deactivate it upon completion
                hitVFX.GetComponent<Renderer>().material.DOFade(0, 1f).OnComplete(() =>
                {
                    hitVFX.SetActive(false);
                    hitVFX.GetComponent<Renderer>().material.DOFade(1, 0); // Reset the fade
                });
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detects damageable enemies when they enter the attack range
        if (other.TryGetComponent<IDamageable>(out var damageable))
            _damageable = damageable;
    }

    private void OnTriggerExit(Collider other)
    {
        // Clears the reference to the damageable enemy when it exits the attack range
        if (other.TryGetComponent<IDamageable>(out var _))
            _damageable = null;
    }
}