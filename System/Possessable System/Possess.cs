using DG.Tweening;
using UnityEngine;

// The Possess class inherits from the Interactable class
public class Possess : Interactable
{
    // A serialized MonoBehaviour field that can be set in the Unity editor
    // This is the script that implements the IPossessable interface
    [SerializeField] private MonoBehaviour _possessableScript;

    // Reference to the InputManager
    private InputManager _inputManager;

    // The original scale of the player
    private Vector3 _originalScale;

    // Called before the first frame update
    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    // The Interact method is overridden from the Interactable class
    protected override void Interact(ObjectType _playerType)
    {
        if (_playerType != null && _playerType.IsGhost)
        {
            _originalScale = _playerType.transform.localScale;

            if (_possessableScript != null)
                if (_possessableScript.TryGetComponent<IPossessable>(out var possessable))
                    possessable.GhostPlayer = _playerType.gameObject;

            _inputManager.GhostActions.Disable();

            _playerType.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.InOutElastic).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f).OnComplete(() =>
            {
                // Additional null checks to prevent null reference exceptions
                if (_playerType == null || _possessableScript == null)
                {
                    Logging.LogError("One or more references (_playerType or _possessableScript) are null in DOTween callback.");
                    return;
                }

                _playerType.transform.localScale = _originalScale;
                base.Interact(_playerType);

                if (_possessableScript.TryGetComponent<IPossessable>(out var possessableAfter))
                    possessableAfter.Possess();
            });
        }
    }
}