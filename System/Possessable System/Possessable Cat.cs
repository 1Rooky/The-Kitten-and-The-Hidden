using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableCat : MonoBehaviour, IPossessable
{
    // Serialized fields that can be set in the Unity editor
    [Header("Movement")]
    [SerializeField] private float _invisibilityAbility = 5f; // Invisibility ability of the cat

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f; // Delay after unpossessing

    [SerializeField] private Collider _collider; // Reference to the Collider component

    private InputManager _inputManager; // Reference to the InputManager

    private Action<InputAction.CallbackContext> _unpossess; // Action to unpossess the cat
    private Action<InputAction.CallbackContext> _startInvisibility; // Action to start invisibility
    private Action<InputAction.CallbackContext> _stoptInvisibility; // Action to stop invisibility

    private WaitForSeconds waitForSeconds; // WaitForSeconds object for delay

    // Property for the ghost player from the IPossessable interface
    GameObject IPossessable.GhostPlayer { get; set; }

    // Called when the object is first initialized
    private void Awake()
    {
        // Initialize the actions
        _unpossess = _ => Unpossess();
        _startInvisibility = _ => StartInvisibility();
        _stoptInvisibility = _ => StopInvisibility();

        // Initialize the Collider component
        if (_collider == null)
            _collider = GetComponent<Collider>();

        // Initialize the WaitForSeconds object
        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);
    }

    // Called before the first frame update
    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    // Called when the object is disabled
    private void OnDisable()
    {
        // Disable the PossessCatActions
        if (_inputManager != null)
            _inputManager.PossessCatActions.Disable();
    }

    // Possess the cat
    public void Possess()
    {
        ((IPossessable)this).GhostPlayer.SetActive(false);

        // Set up the input actions
        _inputManager.PossessCatActions.Unpossess.performed += _unpossess;
        _inputManager.PossessCatActions.Invisibility.performed += _startInvisibility;
        _inputManager.PossessCatActions.Invisibility.canceled += _stoptInvisibility;

        // Enable and disable the appropriate input actions
        _inputManager.GhostActions.Disable();
        _inputManager.PossessCatActions.Enable();
    }

    // Unpossess the cat
    public void Unpossess()
    {
        ((IPossessable)this).GhostPlayer.SetActive(true);

        // Remove the input actions
        _inputManager.PossessCatActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessCatActions.Invisibility.performed -= _startInvisibility;
        _inputManager.PossessCatActions.Invisibility.canceled -= _stoptInvisibility;

        // Enable and disable the appropriate input actions
        _inputManager.GhostActions.Enable();
        _inputManager.PossessCatActions.Disable();

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

    // Start the invisibility
    private void StartInvisibility()
    {
        Logging.Log("Invisibility" + _invisibilityAbility);
    }

    // Stop the invisibility
    private void StopInvisibility()
    {
        Logging.Log("Stop Invisibility" + _invisibilityAbility);
    }
}