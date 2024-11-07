using UnityEngine;
using System.Collections.Generic; // Required for using List

public class CutOutObject : MonoBehaviour
{
    [SerializeField] private LayerMask _obstaclesMask;

    private Transform _catTransform;
    private Transform _ghostTransform;
    private Camera _camera;
    private ServiceLocator _serviceLocator;

    private List<Renderer> _hitCatRenderers = new List<Renderer>();
    private List<Renderer> _hitGhostRenderers = new List<Renderer>();

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        _camera = _serviceLocator.GetService<CameraInstance>().Camera;
        _catTransform = _serviceLocator.GetService<Cat>().GetTransform();
        _ghostTransform = _serviceLocator.GetService<Ghost>().GetTransform();
    }

    private void Update()
    {
        Vector2 _cutOutCatPos = _camera.WorldToViewportPoint(_catTransform.position);
        Vector2 _cutOutGhostPos = _camera.WorldToViewportPoint(_ghostTransform.position);

        Vector3 _catOffset = _catTransform.position - transform.position;
        Vector3 _ghostOffset = _ghostTransform.position - transform.position;

        RaycastHit[] _hitCatObjects = Physics.RaycastAll(transform.position, _catOffset, _catOffset.magnitude, _obstaclesMask);
        RaycastHit[] _hitGhostObjects = Physics.RaycastAll(transform.position, _ghostOffset, _ghostOffset.magnitude, _obstaclesMask);

        if (_hitCatObjects.Length > 0)
        {
            SetMaterialPosition(_hitCatObjects, "_Player1Size", "_Player1Position", _cutOutCatPos, _hitCatRenderers);
        }
        else if(_hitCatRenderers.Count > 0)
        {
            Logging.Log("Test Cat Reset");
            ResetMaterialsPosition("_Player1Size", _hitCatRenderers);
        }

        if (_hitGhostObjects.Length > 0)
        {
            SetMaterialPosition(_hitGhostObjects, "_Player2Size", "_Player2Position", _cutOutGhostPos, _hitGhostRenderers);
        }
        else if(_hitGhostRenderers.Count > 0)
        {
            ResetMaterialsPosition("_Player2Size", _hitGhostRenderers);
        }
    }

    private void SetMaterialPosition(RaycastHit[] _hitObjects, string _matSizeParameter, string _matPositionParameter, Vector2 _cutPos, List<Renderer> renderersList)
    {
        foreach (RaycastHit hit in _hitObjects)
        {
            Renderer hitRenderer = hit.transform.GetComponent<Renderer>();
            if (!renderersList.Contains(hitRenderer))
            {
                renderersList.Add(hitRenderer);
            }
            Material[] _cutoutMat = hitRenderer.materials;
            foreach (Material mat in _cutoutMat)
            {
                mat.SetFloat(_matSizeParameter, 0.55f);
                mat.SetVector(_matPositionParameter, _cutPos);
            }
        }
    }

    private void ResetMaterialsPosition(string _matParameter, List<Renderer> renderersList)
    {
        foreach (Renderer renderer in renderersList)
        {
            Material[] _cutoutMat = renderer.materials;
            foreach (Material mat in _cutoutMat)
            {
                mat.SetFloat(_matParameter, 0);
            }
        }
        renderersList.Clear(); // Clear the list after resetting the materials
    }
}

