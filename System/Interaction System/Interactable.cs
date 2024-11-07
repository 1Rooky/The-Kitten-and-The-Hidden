using DG.Tweening;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

// Defines an abstract base class for objects that can be interacted with in the game.
public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private LayerMask _layersInteractedWith; // Specifies which layers can interact with this object.

    [SerializeField] private Material _outlineMaterial; // Material used to highlight the object when it can be interacted with.
    [SerializeField] private Renderer[] renderers; // Renderers that will have the outline material applied.

    [Header("Interaction Flags")]
    [SerializeField] private bool _interact; // Flag to enable or disable interaction with this object.

    [SerializeField] private bool _autoInteract; // If true, the object will interact automatically without player input.
    [SerializeField] private string _promptMessage; // Message to display as a prompt when the object can be interacted with.

    [Header("Events")]
    [SerializeField] private bool _useEvents; // Determines if Unity Events are used to handle interaction.

    [Header("Particle Effects")]
    [SerializeField] private bool _useParticleEffect = false; // Flag to enable or disable particle effects upon interaction.

    [SerializeField] private ParticleSystem _interactionParticals; // Particle system to play when the object is interacted with.

    [Header("Audio SFX")]
    [SerializeField] private bool _AddSFX = false;

    [SerializeField] private EventReference _sfx;

    private Renderer _renderer; // The renderer that will be used for applying the outline effect.
    private ObjectPool _objectPool; // Reference to an object pool for managing particle systems.

    private AudioSystemFMOD _audioSystem;

    // Public properties to expose private fields for interaction settings.
    public LayerMask LayersInteractedWith { get => _layersInteractedWith; set => _layersInteractedWith = value; }

    public bool InteractProperty { get => _interact; set => _interact = value; }
    public bool AutoInteract { get => _autoInteract; set => _autoInteract = value; }
    public string PromptMessage { get => _promptMessage; set => _promptMessage = value; }
    public bool UseEvents { get => _useEvents; set => _useEvents = value; }
    public bool UseParticleEffect { get => _useParticleEffect; set => _useParticleEffect = value; }
    public ParticleSystem InteractionParticals { get => _interactionParticals; set => _interactionParticals = value; }

    protected AudioSystemFMOD AudioSystem => _audioSystem;

    // Internal properties to manage the materials for the outline effect.
    internal Material[] OriginalMaterials { get; private set; }

    internal Material[] MaterialsWithOutline { get; private set; }

    // Initializes the interactable object, setting up the outline effect.
    private void Awake() => Initialize(_outlineMaterial);

    // Retrieves the object pool service on start.
    private void Start()
    {
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();
        _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
    }

    // Ensures the outline effect is initialized correctly when values are changed in the editor.
    private void OnValidate()
    {
        if (this != null && !Application.isPlaying)
            Initialize(_outlineMaterial);
    }

    // Prepares the materials for the outline effect based on the assigned renderers.
    internal void Initialize(Material outlineMaterial)
    {
        if (AutoInteract || !AutoInteract || !InteractProperty)
            return;

        if (outlineMaterial == null)
        {
            Logging.LogError("outlineMaterial is null");
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                OriginalMaterials = renderer.sharedMaterials;

                List<Material> materialsWithoutDuplicates = new(renderer.sharedMaterials);
                materialsWithoutDuplicates.RemoveAll(material => material == outlineMaterial);

                MaterialsWithOutline = new Material[materialsWithoutDuplicates.Count + 1];
                materialsWithoutDuplicates.CopyTo(MaterialsWithOutline, 0);
                MaterialsWithOutline[^1] = outlineMaterial;

                _renderer = renderer;
                break;
            }
        }
    }

    // Applies or removes the outline effect based on the interaction state.
    internal void ApplyOutline(bool _outlineEnabled)
    {
        if (AutoInteract)
            return;

        if (_outlineEnabled && _renderer != null && _outlineMaterial != null && !System.Array.Exists(_renderer.sharedMaterials, material => material == _outlineMaterial))
            _renderer.sharedMaterials = MaterialsWithOutline;
        else if (!_outlineEnabled)
            RemoveOutline();
    }

    // Removes the outline effect by reverting to the original materials.
    internal void RemoveOutline()
    {
        if (AutoInteract)
            return;

        if (_renderer != null)
            _renderer.sharedMaterials = OriginalMaterials;
    }

    // Base method for handling interaction, checking layer compatibility and invoking the specific interaction logic.
    internal void BaseInteract(ObjectType _objectType)
    {
        if (_objectType == null)
        {
            Logging.LogWarning("ObjectType is null in Interact method.");
            return;
        }

        if (((1 << _objectType.gameObject.layer) & _layersInteractedWith) != 0)
            Interact(_objectType);
        else
            Logging.LogWarning($"Object {_objectType.name} is not interactable");
    }

    // Abstract method to be implemented by derived classes to define specific interaction logic.
    protected virtual void Interact(ObjectType _objectType)
    {
        if (UseParticleEffect)
        {
            if (_objectPool == null)
                _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

            GameObject particleInstanceObject = _objectPool.GetPooledObject(InteractionParticals.gameObject);

            if (particleInstanceObject != null)
            {
                ParticleSystem particleInstance = particleInstanceObject.GetComponent<ParticleSystem>();
                particleInstance.transform.position = _objectType.transform.position;
                particleInstance.Play();

                DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
            }
        }

        if (_AddSFX)
        {
            if (_audioSystem == null)
                _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();

            AudioSystem.PlayerShooting(_sfx, gameObject.transform.position);
        }
        if (_useEvents)
            if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                _events.onInteract.Invoke();
    }
}