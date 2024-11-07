using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    // Serialized fields that can be set in the Unity editor
    [Header("Throwing Mechanism")]
    [SerializeField] private GameObject _ballPrefab; // Prefab for the ball to be thrown

    [SerializeField] private float shootDelay = 1f; // Delay between each shot in seconds
    [SerializeField] private float _baseVelocity = 2f; // Base velocity for the throw
    [SerializeField] private float _velocityMultiplier = 2f; // Velocity multiplier for the throw

    [Header("Trajectory Line Renderer")]
    [SerializeField] private LineRenderer _trajectoryLineRenderer; // Line renderer for the trajectory

    [SerializeField] private int numPoints = 10; // Number of points in the trajectory
    [SerializeField] private float _timeBetweenPoints = 0.1f; // Time between points in the trajectory
    [SerializeField] private float pointIncreaseInterval = 0.2f; // Interval to increase the points in the trajectory

    [Header("Shoot Animation")]
    [SerializeField] private CustomDoTweenAnimation _customInteractionAnimation; // Reference to the custom interaction animation

    private ObjectPool _objectPool; // Reference to the ObjectPool
    private Coroutine _throwCoroutine; // Coroutine for the throw
    private AudioSystemFMOD _audioSystem; // Reference to the AudioSystem
    private InputManager _inputManager; // Reference to the InputManager
    private WaitForSeconds _waitForShootDelay; // WaitForSeconds object for shoot delay

    private Action<InputAction.CallbackContext> _startThrowAction; // Action to start the throw
    private Action<InputAction.CallbackContext> _endThrowAction; // Action to end the throw

    private bool _checkingPlayerInput = false; // Flag to check if the player input is being checked

    private Vector3 _initialVelocity; // Initial velocity for the throw
    private float _currentVelocity; // Current velocity for the throw

    private float _holdTime = 0f; // Time the throw button is held
    private bool _isFireKeyPressed = false; // Flag to check if the fire key is being pressed
    private bool _canShoot = true; // Flag to check if the player can shoot

    // Properties
    protected InputManager InputManager => _inputManager; // Property for the InputManager

    protected AudioSystemFMOD AudioSystem => _audioSystem; // Property for the AudioSystem
    protected Action<InputAction.CallbackContext> StartThrowAction => _startThrowAction; // Property for the start throw action
    protected Action<InputAction.CallbackContext> EndThrowAction => _endThrowAction; // Property for the end throw action

    protected Vector3 InitialVelocity
    { get => _initialVelocity; set { _initialVelocity = value; } } // Property for the initial velocity

    protected float CurrentVelocity
    { get => _currentVelocity; private set { _currentVelocity = value; } } // Property for the current velocity

    protected bool CheckingPlayerInput => _checkingPlayerInput;
    protected float TimeBetweenPoints => _timeBetweenPoints; // Property for the time between points
    protected LineRenderer TrajectoryLineRenderer => _trajectoryLineRenderer; // Property for the trajectory line renderer

    // Called when the object is enabled
    protected virtual void OnEnable()
    {
        // Initialize the WaitForSeconds object
        _waitForShootDelay = new WaitForSeconds(shootDelay);

        // Get the InputManager and ObjectPool from the ServiceLocator
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Get the CustomDoTweenAnimation component
        if (_customInteractionAnimation == null)
            _customInteractionAnimation = GetComponent<CustomDoTweenAnimation>();

        // Initialize the actions
        _startThrowAction = context =>
        {
            _isFireKeyPressed = true;
            _holdTime = 0f;
        };

        _endThrowAction = context =>
        {
            _isFireKeyPressed = false;

            if (_throwCoroutine != null)
            {
                StopCoroutine(_throwCoroutine);
                _throwCoroutine = null;
            }

            if (_holdTime <= 1.0f)
            {
                CurrentVelocity = _baseVelocity * 5;
                _checkingPlayerInput = true;
            }

            Throw();
        };
    }

    // Called when the object is disabled
    protected virtual void OnDisable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (_throwCoroutine != null)
            StopCoroutine(_throwCoroutine);
    }

    private void Start() => _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>(); // Get the AudioSystem

    // Called every frame
    private void Update()
    {
        if (_isFireKeyPressed)
        {
            _holdTime += Time.deltaTime;
            if (_holdTime > 1f)
            {
                _isFireKeyPressed = false;
                _throwCoroutine = StartCoroutine(StartThrow());
            }
        }
    }

    // Start the throw
    protected virtual IEnumerator StartThrow()
    {
        CurrentVelocity = _baseVelocity;

        TrajectoryLineRenderer.enabled = true;
        TrajectoryLineRenderer.positionCount = numPoints;

        float timer = 0f;

        while (true)
        {
            CurrentVelocity += _velocityMultiplier * Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= pointIncreaseInterval && numPoints < 50 && CurrentVelocity >= 4.5)
            {
                numPoints++;
                timer = 0f;
            }

            DrawTrajectory(numPoints);
            yield return null;
        }
    }

    // Draw the trajectory
    protected virtual void DrawTrajectory(int numP)
    {
    }

    // Throw the ball
    protected virtual void Throw()
    {
        if (!_canShoot) return; // If the player can't shoot, return

        GameObject bullet = _objectPool.GetPooledObject(_ballPrefab);

        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bullet.SetActive(true);

            Rigidbody ballRigidbody = bullet.GetComponent<Rigidbody>();
            ballRigidbody.velocity = InitialVelocity;
            _checkingPlayerInput = false;
        }

        CurrentVelocity = 0;
        TrajectoryLineRenderer.enabled = false;
        numPoints = 10;

        // Start the turret shoot animation with the parent's transform
        if (_customInteractionAnimation != null)
            _customInteractionAnimation.StartTurretShootAnimation(transform.parent);

        // Start the ShootDelay coroutine
        StartCoroutine(ShootDelay());
    }

    // Coroutine to add a delay between each shot
    private IEnumerator ShootDelay()
    {
        _canShoot = false; // Set canShoot to false
        yield return _waitForShootDelay; // Wait for shootDelay seconds
        _canShoot = true; // Set canShoot to true
    }
}