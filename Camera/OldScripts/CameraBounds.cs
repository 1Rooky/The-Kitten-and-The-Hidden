using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private ServiceLocator _serviceLocator;
    private Camera _camera;
    private bool _isOutsideViewport;
    private bool _isCoroutineRunning;

    [Header("Players Transform")]
    [SerializeField] private Transform _catTransform;
    [SerializeField] private Transform _ghostTransform;

    private Vector3 _originalGhostPos;
    private Vector3 _originalCatPos;

    private Quaternion _originalCatRot;
    private Quaternion _originalGhostRot;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
    }
    private void Start()
    {
        _camera = _serviceLocator.GetService<CameraInstance>().Camera;

        _originalGhostPos = _catTransform.position;
        _originalCatPos = _ghostTransform.position;

        _originalCatRot = _catTransform.rotation;
        _originalCatRot = _ghostTransform.rotation;
    }
    private void LateUpdate()
    {
        Vector3 CatViewportPos = _camera.WorldToViewportPoint(_catTransform.position);
        Vector3 ghostViewportPos = _camera.WorldToViewportPoint(_ghostTransform.position);

        _isOutsideViewport = CatViewportPos.x < 0 || CatViewportPos.x > 1 ||
                               CatViewportPos.y < 0 || CatViewportPos.y > 1 ||
                               CatViewportPos.z < 0 || ghostViewportPos.x < 0 ||
                               ghostViewportPos.x > 1 || ghostViewportPos.y < 0 ||
                               ghostViewportPos.y > 1 || ghostViewportPos.z < 0;


        if (_isOutsideViewport && !_isCoroutineRunning)
        {
            _isCoroutineRunning = true;
            StartCoroutine(GoToCheckPoint());
        }
    }

    private IEnumerator GoToCheckPoint()
    {
        Logging.Log("OutOfTheFrameAndTheTimerStarted");
        yield return new WaitForSeconds(4.0f);

        _catTransform.position = _originalCatPos;
        _ghostTransform.position = _originalGhostPos;

        _catTransform.rotation = _originalCatRot;
        _ghostTransform.rotation = _originalGhostRot;

        _isCoroutineRunning = false;
        yield return new WaitForSeconds(1.0f);
    }
}
