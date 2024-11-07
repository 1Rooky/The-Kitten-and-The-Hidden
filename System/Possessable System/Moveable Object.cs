using System;
using UnityEngine;
using UnityEngine.InputSystem;

// The MoveableObject class allows objects to be possessed and controlled by the player.
public class MoveableObject : MonoBehaviour, IPossessable
{
    [Header("Movement")]
    [SerializeField] private float _speed = 5f; // Speed at which the object moves

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f; // Delay before the object can be interacted with again after being unpossessed

    [SerializeField] private Collider _collider; // The object's collider component
    [SerializeField] private Rigidbody _rigidbody; // The object's Rigidbody component for physics-based movement

    private InputManager _inputManager; // Reference to the InputManager for handling input

    // Delegates for handling input actions
    private Action<InputAction.CallbackContext> _unpossess;

    private Action<InputAction.CallbackContext> _move;

    private WaitForSeconds waitForSeconds; // WaitForSeconds used for delays

    // Flags to track the object's state
    private bool _isMoving = false;

    private bool _isPossessed = false;

    GameObject IPossessable.GhostPlayer { get; set; } // Reference to the ghost player that can possess this object

    private void Awake()
    {
        // Initialize the input action delegates
        _unpossess = _ => Unpossess();
        _move = _ => { _isMoving = true; };

        // Ensure the collider and rigidbody are set, getting them from the GameObject if not explicitly assigned
        if (_collider == null)
            _collider = GetComponent<Collider>();
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);
    }

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>(); // Retrieve the InputManager instance

    private void OnDisable()
    {
        // Disable input actions when the object becomes inactive
        if (_inputManager != null)
            _inputManager.PossessMovableObjectActions.Disable();
    }

    public void Possess()
    {
        // Handle possession logic
        _isPossessed = true;
        ((IPossessable)this).GhostPlayer.SetActive(false); // Hide the ghost player

        // Subscribe to input actions for unpossessing and moving the object
        _inputManager.PossessMovableObjectActions.Unpossess.performed += _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed += _move;
        _inputManager.PossessMovableObjectActions.Move.canceled += _ => _isMoving = false;

        // Enable and disable appropriate input actions
        _inputManager.GhostActions.Disable();
        _inputManager.PossessMovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        // Handle unpossession logic
        _isPossessed = false;
        ((IPossessable)this).GhostPlayer.SetActive(true); // Show the ghost player again

        // Unsubscribe from input actions
        _inputManager.PossessMovableObjectActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed -= _move;
        _inputManager.PossessMovableObjectActions.Move.canceled -= _ => _isMoving = false;

        // Enable and disable appropriate input actions
        _inputManager.GhostActions.Enable();
        _inputManager.PossessMovableObjectActions.Disable();

        // Temporarily disable the collider and re-enable it after a delay
        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
    }

    private System.Collections.IEnumerator EnableColliderAfterDelay()
    {
        // Coroutine to wait for the specified delay before re-enabling the collider
        yield return waitForSeconds;
        _collider.enabled = true;
    }

    private void FixedUpdate()
    {
        // Move the object based on input if it is possessed and the move action is active
        if (_isPossessed && _isMoving)
        {
            Vector2 inputVector = _inputManager.PossessMovableObjectActions.Move.ReadValue<Vector2>();

            // Convert the 2D input vector into a 3D movement vector
            Vector3 movement = _speed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y);

            // Apply the movement to the object's position
            _rigidbody.MovePosition(_rigidbody.position + movement);
        }
    }
}