using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelVirtualCameras : MonoBehaviour
{
    [SerializeField] private List<CinemachineCamera> _levelcamerasList;

    private readonly Dictionary<int, CinemachineCamera> _levelcameras = new();

    private void Awake() => ServiceLocator.Instance.RegisterService(this, false);

    private void Start() => AddCamsToDictionary();

    private void AddCamsToDictionary()
    {
        for (int i = 0; i < _levelcamerasList.Count; i++)
            _levelcameras.Add(i + 1, _levelcamerasList[i]);
    }

    internal void CloseCamera(int _key)
    {
        if (_levelcameras.ContainsKey(_key))
            _levelcameras[_key].gameObject.SetActive(false);
    }

    internal void CloseAllCamera()
    {
        foreach (CinemachineCamera cam in _levelcameras.Values)
            cam.gameObject.SetActive(false);
    }

    internal void OpenCamera(int _key)
    {
        if (_levelcameras.ContainsKey(_key))
            _levelcameras[_key].gameObject.SetActive(true);
    }
}