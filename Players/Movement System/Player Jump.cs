using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    private InputManager _inputManager;
    private Action<InputAction.CallbackContext> _jumpAction;

    private Rigidbody rb;
    private int jumpCount = 0;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float maxForce = 15f;
    private float VelocityNode;

    [Header("CatAnimation")]
    [SerializeField] private float animationPlayTransition = 0.001f;
    private Animator _animator;
    private int JumpAnimationId;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistanceNormal = 0.1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        JumpAnimationId = Animator.StringToHash(GameConstant.CatAnimation.IsJumping);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _jumpAction = _ => Jump();
        _inputManager.CatActions.Jump.performed += _jumpAction;
    }

    private void OnDisable() => _inputManager.CatActions.Jump.performed -= _jumpAction;

    private void FixedUpdate()
    {
        ApplyGravity();
        LimitVelocity();
    }

    private void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistanceNormal))
        {
            if (hit.distance < groundCheckDistanceNormal)
            {
                jumpCount = 0;
            }
        }
        if (jumpCount < 2)
        {
            VelocityNode = (2 * gravity * jumpHeight) / jumpDuration;
            rb.velocity = new Vector3(rb.velocity.x, VelocityNode, rb.velocity.z);
            _animator.CrossFade(JumpAnimationId, animationPlayTransition);
            jumpCount++;
            Logging.Log("Jumping");
        }
    }


    private void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    private void LimitVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
        if (rb.velocity.magnitude > maxForce)
        {
            rb.velocity = rb.velocity.normalized * maxForce;
        }
    }

    public void LoadGame(GameData _gameData)
    {
        transform.position = _gameData._playerPosition;
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData._playerPosition = transform.position;
    }
}
