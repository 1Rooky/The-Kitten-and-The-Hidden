using UnityEngine;

[RequireComponent(typeof(ObjectType))]
public class Interact : MonoBehaviour
{
    // Serialized field that can be set in the Unity editor
    [SerializeField] private LayerMask _interactableLayerMask; // LayerMask to identify interactable objects

    private InputManager _inputManager; // Reference to the InputManager
    private UISystem _playerUI; // Reference to the UISystem
    private Interactable currentInteractable; // The current interactable object
    private ObjectType _objectType; // The type of the player

    private ServiceLocator serviceLocator; // Reference to the ServiceLocator

    private bool _hasPlayedInteractSFX = false; // Flag to check if the interact sound effect has been played
    private bool _outlineEnabled = true; // Flag to check if the outline is enabled

    // Property for the interactable layer mask
    internal LayerMask InteractableLayerMask => _interactableLayerMask;

    // Called when the object is first initialized
    private void Awake() => _objectType = GetComponent<ObjectType>();

    // Called before the first frame update
    private void Start()
    {
        serviceLocator = ServiceLocator.Instance;

        // Get the UISystem and InputManager from the ServiceLocator
        _inputManager = serviceLocator.GetService<InputManager>();

        if (serviceLocator.TryGetService<UISystem>(out var _UISystem))
        {
            _playerUI = _UISystem;
            _playerUI.DisablePromptText();
        }

        // Check the player type and set up the interact action
        if (_objectType != null)
        {
            if (_objectType.IsCat)
                _inputManager.CatActions.Interact.performed += _ => StartInteraction();
            else if (_objectType.IsGhost)
                _inputManager.GhostActions.Interact.performed += _ => StartInteraction();
            else if (_objectType.IsObject)
                Logging.Log($"Object {gameObject.name} is interactable");
            else
                Logging.LogError($"Object {gameObject.name} Type not set!");
        }
    }

    // Called when the object is destroyed
    private void OnDestroy()
    {
        // Check the player type and remove the interact action
        if (_objectType != null)
        {
            if (_objectType.IsCat)
                _inputManager.CatActions.Interact.performed -= _ => StartInteraction();
            else if (_objectType.IsGhost)
                _inputManager.GhostActions.Interact.performed -= _ => StartInteraction();
            else if (_objectType.IsObject)
                Logging.Log($"Object {gameObject.name} is interactable");
            else
                Logging.LogError($"Object {gameObject.name} Type not set!");
        }
    }

    // Called when the object enters a trigger
    private void OnTriggerEnter(Collider other)
    {
        // If the other object is interactable
        if (((1 << other.gameObject.layer) & InteractableLayerMask) != 0)
        {
            // If the other object has an Interactable component
            if (other.TryGetComponent<Interactable>(out var _interactable))
            {
                currentInteractable = _interactable;
                _outlineEnabled = true;
                currentInteractable.ApplyOutline(_outlineEnabled);
            }

            // If the interactable object does not auto interact
            if (!_interactable.AutoInteract)
                _playerUI.UpdatePromptText(_interactable.PromptMessage);
            else
                StartInteraction();
        }
    }

    // Called when the object exits a trigger
    private void OnTriggerExit(Collider other)
    {
        // If the other object is interactable
        if (((1 << other.gameObject.layer) & InteractableLayerMask) != 0)
            ResetInteraction();
    }

    private void ResetInteraction()
    {
        _outlineEnabled = false;
        _hasPlayedInteractSFX = false;

        if (currentInteractable != null)
            currentInteractable.ApplyOutline(_outlineEnabled);

        SetCurrentInteractableToNull();
    }

    // Set the current interactable object to null
    private void SetCurrentInteractableToNull()
    {
        if (currentInteractable == null)
            return;

        if (_playerUI != null)
            _playerUI.DisablePromptText();

        currentInteractable.RemoveOutline();
        currentInteractable = null;
    }

    // Start the interaction with the current interactable object
    private void StartInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.RemoveOutline();
            bool shouldInteract = currentInteractable.AutoInteract || _inputManager.CatActions.Interact.triggered || _inputManager.GhostActions.Interact.triggered;

            if (shouldInteract)
            {
                if (!_hasPlayedInteractSFX)
                {
                    //_audioManager.PlaySFX(_audioManager._interact);
                    _hasPlayedInteractSFX = true;
                }

                currentInteractable.BaseInteract(_objectType);
            }
        }

        // Reset the interaction
        ResetInteraction();
    }
}