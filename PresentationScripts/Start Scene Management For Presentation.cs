using UnityEngine;

public class StartSceneManagementForPresentation : MonoBehaviour
{
    [SerializeField] private SceneManagement _sceneManagement;

    private void Awake() => _sceneManagement.StartLevel();
}