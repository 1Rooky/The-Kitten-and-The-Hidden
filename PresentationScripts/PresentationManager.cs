using System.Collections.Generic;
using UnityEngine;

public class PresentationManager : MonoBehaviour
{
    [SerializeField] private List<PresentationCamera> _cameras;

    private int _currentCam = 0;

    private void Update()
    {
        if (_cameras != null)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (_currentCam < _cameras.Count - 1)
                {
                    _currentCam++;
                    CloseAllCamera();
                    OpenCamera();
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (_currentCam > 0 && _currentCam <= _cameras.Count)
                {
                    _currentCam--;
                    CloseAllCamera();
                    OpenCamera();
                }
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _cameras[_currentCam].CamKeyEvent?.Invoke();
            }
        }
    }

    private void CloseAllCamera()
    {
        foreach (PresentationCamera cam in _cameras)
        {
            cam.gameObject.SetActive(false);
        }
    }

    private void OpenCamera()
    {
        _cameras[_currentCam].gameObject.SetActive(true);
        _cameras[_currentCam].CamEvent?.Invoke();
    }
}