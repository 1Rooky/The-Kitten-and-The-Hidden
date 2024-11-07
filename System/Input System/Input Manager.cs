using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem _input;
    internal InputSystem.CatActions CatActions { get; private set; }
    internal InputSystem.GhostActions GhostActions { get; private set; }
    internal InputSystem.PossessTurretActions PossessTurretActions { get; private set; }
    internal InputSystem.PossessMovableObjectActions PossessMovableObjectActions { get; private set; }
    internal InputSystem.PossessCatActions PossessCatActions { get; private set; }
    internal InputSystem.DialogActions DialogActions { get; private set; }
    internal InputSystem.PauseActions PauseActions { get; private set; }

    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        _input = new InputSystem();

        // Initialize actions
        InitializeActions();

        // Disable some actions initially
        DisableSomeActions();
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input?.Disable();

    private void InitializeActions()
    {
        CatActions = _input.Cat;
        GhostActions = _input.Ghost;
        PossessTurretActions = _input.PossessTurret;
        PossessMovableObjectActions = _input.PossessMovableObject;
        PossessCatActions = _input.PossessCat;
        DialogActions = _input.Dialog;
        PauseActions = _input.Pause;
    }

    private void DisableSomeActions()
    {
        PossessTurretActions.Disable();
        PossessMovableObjectActions.Disable();
        PossessCatActions.Disable();
        DialogActions.Disable();
    }

    // Disable all inputs except Pause
    internal void DisableAllInputsExceptPause()
    {
        CatActions.Disable();
        GhostActions.Disable();
        PossessTurretActions.Disable();
        PossessMovableObjectActions.Disable();
        PossessCatActions.Disable();
        DialogActions.Disable();
    }

    // Enable all inputs
    internal void EnableAllInputs()
    {
        // Re-enable all input actions here
        CatActions.Enable();
        GhostActions.Enable();
        PossessTurretActions.Enable();
        PossessMovableObjectActions.Enable();
        PossessCatActions.Enable();
        DialogActions.Enable();
        PauseActions.Enable();
    }
}