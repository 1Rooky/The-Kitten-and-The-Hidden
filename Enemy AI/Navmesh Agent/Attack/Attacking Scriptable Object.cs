using UnityEngine;

// Defines a ScriptableObject for configuring enemy attacks, allowing for easy adjustments and reuse across different enemies
[CreateAssetMenu(fileName = "Attacking Configuration", menuName = "Enemy Scriptable Object/Attacking Configuration")]
public class AttackingScriptableObject : ScriptableObject
{
    [Header("Attack Configuration")]
    [SerializeField] private int _damage = 100; // The damage dealt by the attack

    [SerializeField] private float _attackRadius = 1.5f; // The radius within which the attack is effective
    [SerializeField] private float _attackDelay = 1.5f; // The delay between attacks to prevent spamming

    [Header("Ranged Configuration")]
    [SerializeField] private bool _isRanged = false; // Flag to determine if the attack is ranged

    [SerializeField] private bool _isHomingBullet = false; // Flag for whether the bullet should home in on targets
    [SerializeField] private GameObject _bulletPrefab; // The prefab to use for bullets in ranged attacks
    [SerializeField] private Vector3 _bulletSpawnOffset = new(0, 1, 0); // Offset for spawning bullets relative to the attacker
    [SerializeField] private LayerMask _lineOfSightLayers = -1; // LayerMask to determine what the attack can hit or be blocked by

    #region Getters & Setters

    // Properties for accessing and modifying the attack configuration
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public float AttackRadius
    {
        get { return _attackRadius; }
        set { _attackRadius = value; }
    }

    public float AttackDelay
    {
        get { return _attackDelay; }
        set { _attackDelay = value; }
    }

    public bool IsRanged
    {
        get { return _isRanged; }
        set { _isRanged = value; }
    }

    public bool IsHomingBullet
    {
        get { return _isHomingBullet; }
        set { _isHomingBullet = value; }
    }

    public GameObject BulletPrefab
    {
        get { return _bulletPrefab; }
        set { _bulletPrefab = value; }
    }

    public Vector3 BulletSpawnOffset
    {
        get { return _bulletSpawnOffset; }
        set { _bulletSpawnOffset = value; }
    }

    public LayerMask LineOfSightLayers
    {
        get { return _lineOfSightLayers; }
        set { _lineOfSightLayers = value; }
    }

    #endregion Getters & Setters

    // Method to apply the configured attack properties to an enemy instance
    public void SetupEnemy(Enemy enemy)
    {
        // Apply basic attack properties
        enemy.AttackRadius.SphereColliderAttackRadius = _attackRadius;
        enemy.AttackRadius.AttackDelay = _attackDelay;
        enemy.AttackRadius.Damage = _damage;

        // If the attack is ranged, configure the ranged attack properties
        if (_isRanged)
        {
            if (enemy.AttackRadius.TryGetComponent<RangedAttackRadius>(out var rangedAttackRadius))
            {
                if (BulletPrefab == null)
                    return; // Exit if no bullet prefab is set

                // Apply ranged attack properties
                rangedAttackRadius.Bullet = BulletPrefab.GetComponent<Bullet>();
                rangedAttackRadius.UseHomingBullet = IsHomingBullet;
                rangedAttackRadius.BulletSpawnOffset = BulletSpawnOffset;
                rangedAttackRadius.LineOfSightLayers = LineOfSightLayers;
            }
        }
    }
}