using System;
using UnityEngine;
using UnityEngine.InputSystem;

// The PossessableTurret class implements the IPossessable interface
public class PossessableTurret : MonoBehaviour, IPossessable
{
    // Serialized fields are private variables that can be set in the Unity editor
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation

    [SerializeField] private Vector2 _angleLimitForRotation; // Limit for rotation angle
    [SerializeField] private TurretThrowing _turretThrowing; // Reference to the TurretThrowing component

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f; // Delay after unpossessing

    [SerializeField] private Collider _collider; // Reference to the Collider component

    private bool _isPossessed = false; // Flag to check if the turret is possessed
    private Vector2 _inputVector; // Input vector for rotation
    private Vector3 currentRotation; // Current rotation of the turret

    private InputManager _inputManager; // Reference to the InputManager
    private Action<InputAction.CallbackContext> _unpossess; // Action to unpossess the turret

    private WaitForSeconds waitForSeconds; // WaitForSeconds object for delay

    // Property for the ghost player from the IPossessable interface
    GameObject IPossessable.GhostPlayer { get; set; }

    // Called when the object is first initialized
    private void Awake()
    {
        // Initialize the TurretThrowing and Collider components
        if (_turretThrowing == null)
            _turretThrowing = GetComponentInChildren<TurretThrowing>();

        if (_collider == null)
            _collider = GetComponent<Collider>();

        // Initialize the WaitForSeconds object
        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);

        // Initialize the unpossess action
        _unpossess = _ => Unpossess();
    }

    // Called before the first frame update
    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    // Called when the object is disabled
    private void OnDisable()
    {
        // Disable the TurretThrowing component and the PossessTurretActions
        if (_turretThrowing != null)
            _turretThrowing.enabled = false;

        // Disable the PossessTurretActions
        if (_inputManager != null)
            _inputManager.PossessTurretActions.Disable();
    }

    // Possess the turret
    public void Possess()
    {
        _isPossessed = true;
        _turretThrowing.enabled = true;
        ((IPossessable)this).GhostPlayer.SetActive(false);

        // Set up the input actions
        _inputManager.PossessTurretActions.Unpossess.performed += _unpossess;
        _inputManager.PossessTurretActions.Look.performed += ctx => { _inputVector = ctx.ReadValue<Vector2>(); };
        _inputManager.PossessTurretActions.Look.canceled += _ => { _inputVector = Vector2.zero; };

        // Enable and disable the appropriate input actions
        _inputManager.GhostActions.Disable();
        _inputManager.PossessTurretActions.Enable();
    }

    // Unpossess the turret
    public void Unpossess()
    {
        _isPossessed = false;
        _turretThrowing.enabled = false;
        ((IPossessable)this).GhostPlayer.SetActive(true);

        // Remove the input actions
        _inputManager.PossessTurretActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessTurretActions.Look.performed -= ctx => { _inputVector = ctx.ReadValue<Vector2>(); };
        _inputManager.PossessTurretActions.Look.canceled -= _ => { _inputVector = Vector2.zero; };

        // Enable and disable the appropriate input actions
        _inputManager.GhostActions.Enable();
        _inputManager.PossessTurretActions.Disable();

        // Disable the collider and enable it after a delay
        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
    }

    // Enable the collider after a delay
    private System.Collections.IEnumerator EnableColliderAfterDelay()
    {
        yield return waitForSeconds;
        _collider.enabled = true;
    }

    // Called every frame
    private void Update()
    {
        // If the turret is possessed and the input vector is not zero
        if (_isPossessed && _inputVector != Vector2.zero)
        {
            // Calculate the rotation amount based on the input vector and the rotation speed
            Vector3 rotationAmount = rotationSpeed * Time.deltaTime * new Vector3(-_inputVector.y, _inputVector.x, 0);

            // Add the rotation amount to the current rotation
            currentRotation += rotationAmount;

            // Clamp the rotation angles
            currentRotation.x = Mathf.Clamp(currentRotation.x, _angleLimitForRotation.x, _angleLimitForRotation.y);
            currentRotation.y = Mathf.Clamp(currentRotation.y, _angleLimitForRotation.x, _angleLimitForRotation.y);

            // Apply the rotation to the turret
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }
}