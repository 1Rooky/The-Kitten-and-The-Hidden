using UnityEngine;
using UnityEngine.AI;

// This ScriptableObject defines the configuration for enemy characters, including AI behavior and NavMeshAgent properties.
[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "Enemy Scriptable Object/Enemy Configuration Type")]
public class EnemyScriptableObject : ScriptableObject
{
    #region Properties

    [Header("Enemy")]
    [SerializeField] private AttackingScriptableObject _attackingConfiguration; // Configuration for enemy's attacking behavior

    [SerializeField] private float _AIUpdateInterval = 0.1f; // How often the AI updates its state

    [Header("Enemy States")]
    [SerializeField] private EnemyStates _defaultState; // The default state of the enemy

    [SerializeField] private float _idleLocationRadius = 4f; // Radius within which the enemy can move while idle
    [SerializeField] private float _idleMovingSpeedMultiplier = 0.5f; // Speed multiplier for moving while idle
    [SerializeField][Range(2, 10)] private int _wayPoints = 4; // Number of waypoints for patrolling
    [SerializeField] private float _fieldOfView = 90f; // Field of view for detecting the player
    [SerializeField] private float _lineOfSightRange = 6f; // Range for line of sight detection

    [Header("Navmesh Agent")]
    [SerializeField] private float _acceleration = 8; // Acceleration of the NavMeshAgent

    [SerializeField] private float _angularSpeed = 120; // Angular speed of the NavMeshAgent
    [SerializeField] private int _areamask = -1; // Area mask for the NavMeshAgent

    [SerializeField] private int _avoidancePriority = 50; // Avoidance priority for the NavMeshAgent
    [SerializeField] private float _baseOffset = 0f; // Base offset for the NavMeshAgent
    [SerializeField] private float _height = 2f; // Height of the NavMeshAgent

    [SerializeField] private ObstacleAvoidanceType _obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance; // Obstacle avoidance type for the NavMeshAgent

    [SerializeField] private float _radius = 0.5f; // Radius of the NavMeshAgent
    [SerializeField] private float _speed = 3f; // Speed of the NavMeshAgent
    [SerializeField] private float _stoppingDistance = 0.5f; // Stopping distance for the NavMeshAgent

    #endregion Properties

    #region Getters

    // Getters for accessing the properties
    internal AttackingScriptableObject AttackingConfiguration => _attackingConfiguration;

    internal float AIUpdateInterval => _AIUpdateInterval;
    internal float Acceleration => _acceleration;
    internal float AngularSpeed => _angularSpeed;
    internal int Areamask => _areamask;
    internal int AvoidancePriority => _avoidancePriority;
    internal float BaseOffset => _baseOffset;
    internal float Height => _height;
    internal ObstacleAvoidanceType ObstacleAvoidanceType => _obstacleAvoidanceType;
    internal float Radius => _radius;
    internal float Speed => _speed;
    internal float StoppingDistance => _stoppingDistance;

    #endregion Getters

    // Method to apply the configuration to an enemy instance
    public void SetupEnemy(Enemy enemy)
    {
        // Apply attacking configuration
        _attackingConfiguration.SetupEnemy(enemy);

        // Apply AI and movement configurations
        enemy.NavmeshEnemyMovment.UpdateSpeed = AIUpdateInterval;
        enemy.NavmeshEnemyMovment.DefaultState = _defaultState;
        enemy.NavmeshEnemyMovment.IdleLocationRadius = _idleLocationRadius;
        enemy.NavmeshEnemyMovment.IdleMovingSpeedMultiplier = _idleMovingSpeedMultiplier;
        enemy.NavmeshEnemyMovment.WayPointsIndex = new Vector3[_wayPoints];

        // Apply line of sight configuration
        enemy.NavmeshEnemyMovment.LineOfSightChecker.FieldOfView = _fieldOfView;
        enemy.NavmeshEnemyMovment.LineOfSightChecker.ColliderRadius = _lineOfSightRange;
        enemy.NavmeshEnemyMovment.LineOfSightChecker.LineOfSightLayers = AttackingConfiguration.LineOfSightLayers;

        // Apply NavMeshAgent configurations
        enemy.NavMeshAgent.acceleration = Acceleration;
        enemy.NavMeshAgent.angularSpeed = AngularSpeed;
        enemy.NavMeshAgent.areaMask = Areamask;
        enemy.NavMeshAgent.avoidancePriority = AvoidancePriority;
        enemy.NavMeshAgent.baseOffset = BaseOffset;
        enemy.NavMeshAgent.height = Height;
        enemy.NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.NavMeshAgent.radius = Radius;
        enemy.NavMeshAgent.speed = Speed;
        enemy.NavMeshAgent.stoppingDistance = StoppingDistance;
    }
}