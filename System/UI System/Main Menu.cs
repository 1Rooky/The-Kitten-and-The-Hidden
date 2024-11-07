using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _continueGame;

    [SerializeField] private Button _newGame;
    [SerializeField] private Button _loadGame;

    private ServiceLocator _serviceLocator;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        bool _hasGameStarted = _serviceLocator.GetService<DataPersistenceManager>().HasGameStarted();

        _continueGame.interactable = _hasGameStarted;
        _loadGame.interactable = _hasGameStarted;
    }

    public void OnContiuneGameClicked()
    {
        _serviceLocator.GetService<DataPersistenceManager>().SaveGame();
        _serviceLocator.GetService<SceneManagement>().StartLevel();
    }

    public void OnExitClicked() => Application.Quit();
}