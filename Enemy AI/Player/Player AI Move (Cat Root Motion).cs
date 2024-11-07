using KinematicCharacterController;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewMovmentSystemRootMotion : MonoBehaviour, ICharacterController
{
    #region Parameters

    [Header("Air Movement")]
    [SerializeField] private float MaxAirMoveSpeed = 10f;

    [SerializeField] private float AirAccelerationSpeed = 5f;
    [SerializeField] private float Drag = 0.1f;

    [Header("Misc")]
    [SerializeField] private Vector3 Gravity = new(0, -30f, 0);

    [SerializeField] private Transform MeshRoot;

    private Animator _animator;
    private KinematicCharacterMotor _motor;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;
    private Action<InputAction.CallbackContext> _jumpAction;

    #region Animation Parameters

    private int RunAnimationId;

    private Vector3 _rootMotionPositionDelta;
    private Quaternion _rootMotionRotationDelta;

    #endregion Animation Parameters

    private float _forwardAxis;

    #endregion Parameters

    private void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.AnimationTest.HorizontalMove);

        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        #region Input Actions

        _startMoveAction = _ =>
        {
        };

        _stopMoveAction = _ =>
        {
        };

        _jumpAction = _ =>
        {
        };

        _inputManager.CatActions.Move.started += _startMoveAction;
        _inputManager.CatActions.Move.canceled += _stopMoveAction;
        _inputManager.CatActions.Jump.started += _jumpAction;

        #endregion Input Actions
    }

    private void Start() => _motor.CharacterController = this;

    private void OnDisable()
    {
        _inputManager.CatActions.Move.started -= _startMoveAction;
        _inputManager.CatActions.Move.canceled -= _stopMoveAction;
    }

    private void Update()
    {
        Vector2 inputVector = _inputManager.CatActions.Move.ReadValue<Vector2>();

        _animator.SetFloat(RunAnimationId, inputVector.magnitude);
    }

    private void OnAnimatorMove()
    {
        Logging.Log("OnAnimatorMove called, deltaPosition: " + _animator.deltaPosition);
        _rootMotionPositionDelta += _animator.deltaPosition;
        _rootMotionRotationDelta = _animator.deltaRotation * _rootMotionRotationDelta;
    }

    #region ICharacterController

    public void AfterCharacterUpdate(float deltaTime)
    {
        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentRotation = _rootMotionRotationDelta * currentRotation;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            if (deltaTime > 0)
            {
                // The final velocity is the velocity from root motion reoriented on the ground plane
                currentVelocity = _rootMotionPositionDelta / deltaTime;
                currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, _motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
            }
            else
            {
                // Prevent division by zero
                currentVelocity = Vector3.zero;
            }
        }
        else
        {
            if (_forwardAxis > 0f)
            {
                // If we want to move, add an acceleration to the velocity
                Vector3 targetMovementVelocity = _forwardAxis * MaxAirMoveSpeed * _motor.CharacterForward;
                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                currentVelocity += AirAccelerationSpeed * deltaTime * velocityDiff;
            }

            // Gravity
            currentVelocity += Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }
    }

    #endregion ICharacterController
}