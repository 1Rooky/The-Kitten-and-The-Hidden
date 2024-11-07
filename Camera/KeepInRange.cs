using UnityEngine;
using UnityEngine.Events;

public class KeepInRange : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [Range(0, 1)][SerializeField] private float _dangerDistancePrecentage;

    private Transform _catTransform;
    private Transform _ghostTransform;
    private Camera _camera;

    private ServiceLocator _serviceLocator;

    private float _dangerDistance;
    private bool inDanger;
    private bool maxDistanceReached;

    private UnityAction _onMaxDistanceReached;
    private UnityAction _onDanger;

    internal UnityAction OnMaxDistanceReached
    { set { _onMaxDistanceReached = value; } get { return _onMaxDistanceReached; } }

    internal UnityAction OnDanger
    { set { _onDanger = value; } get { return _onDanger; } }

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, false);
    }

    private void OnDestroy() => _serviceLocator.UnregisterService<KeepInRange>();

    private void Start()
    {
        _catTransform = _serviceLocator.GetService<Cat>().GetTransform();
        _ghostTransform = _serviceLocator.GetService<Ghost>().GetTransform();
        _camera = _serviceLocator.GetService<CameraInstance>().Camera;
        _dangerDistance = _maxDistance * _dangerDistancePrecentage;
    }

    private void LateUpdate()
    {
        if (IsOutsideViewport())
        {
            float _currentDistance = Vector3.Distance(_catTransform.position, _ghostTransform.position);
            if (!maxDistanceReached || !inDanger)
            {
                if (_currentDistance >= _maxDistance)
                {
                    maxDistanceReached = true;
                    _onMaxDistanceReached?.Invoke();
                }
                else if (_currentDistance >= _dangerDistance && !inDanger)
                {
                    inDanger = true;
                    _onDanger?.Invoke();
                }
            }
            else if (inDanger && _currentDistance < _dangerDistance)
                inDanger = false;
        }
    }

    internal void ResetValues()
    {
        inDanger = false;
        maxDistanceReached = false;
    }

    private bool IsOutsideViewport()
    {
        if (_catTransform == null)
            _catTransform = _serviceLocator.GetService<Cat>().GetTransform();

        if (_ghostTransform == null)
            _ghostTransform = _serviceLocator.GetService<Ghost>().GetTransform();

        Vector3 CatViewportPos = _camera.WorldToViewportPoint(_catTransform.position);
        Vector3 ghostViewportPos = _camera.WorldToViewportPoint(_ghostTransform.position);

        bool isOut = CatViewportPos.x < 0 || CatViewportPos.x > 1 ||
                                   CatViewportPos.y < 0 || CatViewportPos.y > 1 ||
                                   CatViewportPos.z < 0 || ghostViewportPos.x < 0 ||
                                   ghostViewportPos.x > 1 || ghostViewportPos.y < 0 ||
                                   ghostViewportPos.y > 1 || ghostViewportPos.z < 0;
        return isOut;
    }

    public void ChangeMaxDistance(float distance)
    {
        _maxDistance = distance;
        _dangerDistance = distance * _dangerDistancePrecentage;

        ResetValues();
    }
}