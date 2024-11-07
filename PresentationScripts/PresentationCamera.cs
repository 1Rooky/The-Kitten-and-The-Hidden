using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PresentationCamera : MonoBehaviour
{
    [SerializeField] private UnityEvent _camEvent;
    [SerializeField] private UnityEvent _camKeyEvent;
    private CinemachineCamera _camera;
    public UnityEvent CamEvent => _camEvent;
    public UnityEvent CamKeyEvent => _camKeyEvent;

    public CinemachineCamera Camera => _camera;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
    }
}
