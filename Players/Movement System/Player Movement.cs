using FMOD.Studio;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Parameters

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    private InputManager _inputManager;
    private Rigidbody rb;
    private Camera mainCamera;
    [SerializeField] private bool isCat;
    [SerializeField] private float gravity = -9.81f;

    [Header("CatAnimation")]
    [SerializeField] private float smooth = 5f;
    private Animator _animator;
    private int RunAnimationId;
    private float p_anim;

    [Header("Movement")]
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header ("Steps")]
    [SerializeField] private float startRay = 0.2f;
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private float stepSmooth = 15f;
    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float maxClimbHeight = 0.3f;
    private Vector2 input;
    private bool isMoving;

    [Header("Audio")]
    private EventInstance PlayerFootSteps;
    private AudioSystemFMOD AudioSystem;
    private FMODEvents FmodSystem;

    #endregion Parameters

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);
    }

    private void OnEnable()
    {
        _startMoveAction = _ =>
        {
            if (isCat)
            {
                PlayerFootSteps.getPlaybackState(out PLAYBACK_STATE playbackstate);
                PlayerFootSteps.start();
            }

            isMoving = true;
        };

        _stopMoveAction = _ =>
        {
            isMoving = false;
            StartCoroutine(StopMoveSmoothly());
            rb.velocity = Vector3.zero;
            PlayerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
        };

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (isCat)
        {
            _inputManager.CatActions.Move.started += _startMoveAction;
            _inputManager.CatActions.Move.canceled += _stopMoveAction;
        }
        else
        {
            _inputManager.GhostActions.Move.started += _startMoveAction;
            _inputManager.GhostActions.Move.canceled += _stopMoveAction;
        }
    }

    private void Start()
    {
        AudioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
        FmodSystem = ServiceLocator.Instance.GetService<FMODEvents>();

        PlayerFootSteps = AudioSystem.CreateEventInstance(FmodSystem.PlayerSteps);

        mainCamera = Camera.main;
    }

    private void OnDisable()
    {
        if (isCat)
        {
            _inputManager.CatActions.Move.started -= _startMoveAction;
            _inputManager.CatActions.Move.canceled -= _stopMoveAction;
        }
        else
        {
            _inputManager.GhostActions.Move.started -= _startMoveAction;
            _inputManager.GhostActions.Move.canceled -= _stopMoveAction;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            if (isCat)
                input = _inputManager.CatActions.Move.ReadValue<Vector2>().normalized;
            else
                input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;

            Move();
            Steps();
        }
    }

    private void Steps()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = transform.forward;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        bool stepDetected = false;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            float heightDifference = hit.point.y - transform.position.y;
            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                Vector3 stepUpPosition = new Vector3(transform.position.x, hit.point.y + stepHeight, transform.position.z);
                rb.MovePosition(Vector3.Lerp(rb.position, stepUpPosition, stepSmooth));
                stepDetected = true;
            }
        }
        if (!stepDetected)
        {
            rb.AddForce(Vector3.up * gravity);
        }
    }

    private void Move()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 newVelocity = moveDirection * Speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }

        Animate(input);
    }

    private void Animate(Vector2 input)
    {
        if (p_anim < input.x || p_anim >= input.x)
        {
            p_anim += Time.deltaTime * smooth;
        }

        p_anim = Mathf.Clamp(p_anim, 0.0f, 0.25f);
        _animator.SetFloat(RunAnimationId, p_anim);
    }

    private IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(1, 0, elapsedTime / duration);
            _animator.SetFloat(RunAnimationId, value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetFloat(RunAnimationId, 0);
        p_anim = 0;
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
