using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour, IDataPersistence
{
    [SerializeField] internal bool autoStartLevel = false;
    [SerializeField] private List<Level> levels = new();

    private int currentLevel = 1;
    private List<string> levelScenes;

    private InputManager inputManager;
    private DataPersistenceManager dataPersistenceManager;

    public delegate void LevelLoadingDelegate(bool isLoading, float targetProgress, float duration);

    public event LevelLoadingDelegate OnLevelLoading;

    internal int CurrentLevel
    { get => currentLevel; set { currentLevel = value; } }

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);

    private void Start()
    {
        if (ServiceLocator.Instance.TryGetService<InputManager>(out var _inputManager))
            inputManager = _inputManager;

        if (ServiceLocator.Instance.TryGetService<DataPersistenceManager>(out var _dataPersistenceManager))
            dataPersistenceManager = _dataPersistenceManager;

        // Check if autoStartLevel is true, then call StartLevel
        if (autoStartLevel)
            StartLevel(1);
    }

    public void StartLevel(int _targetLevel = -1)
    {
        if (_targetLevel != -1)
            currentLevel = _targetLevel;
        else
            currentLevel++;

        levelScenes = GetLevelScenes(currentLevel);

        if (levelScenes == null || levelScenes.Count == 0)
        {
            Logging.LogError("No scenes found for level " + currentLevel);

            currentLevel--;
            OnLevelLoading?.Invoke(false, 1f, 5f);
            return;
        }

        if (currentLevel == 1 && inputManager != null)
            inputManager.PauseActions.Disable();

        OnLevelLoading?.Invoke(true, 1f, 5f);

        if (autoStartLevel)
            StartActualLoadingImmediate(levelScenes);
        else
            StartCoroutine(StartActualLoading(levelScenes));            // Start the actual scene loading after a delay
    }

    private List<string> GetLevelScenes(int levelNumber)
    {
        foreach (Level level in levels)
            if (level.levelNumber == levelNumber)
                return level.scenes;

        return null;
    }

    private void StartActualLoadingImmediate(List<string> scenes)
    {
        if (scenes.Count > 0)
        {
            SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Single);

            for (int i = 1; i < scenes.Count; i++)
                SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);

            // Notify UISystem that loading ends before allowing scene activation
            OnLevelLoading?.Invoke(false, 1f, 5f); // This line should already be in place
        }
    }

    private IEnumerator StartActualLoading(List<string> scenes)
    {
        yield return new WaitForSeconds(5f); // Wait for the simulated loading to complete

        if (scenes.Count > 0)
        {
            SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Single);

            for (int i = 1; i < scenes.Count; i++)
                SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);

            // Notify UISystem that loading ends before allowing scene activation
            OnLevelLoading?.Invoke(false, 1f, 5f); // This line should already be in place
        }
    }

    public void LoadGame(GameData _gameData)
    {
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData.SetTotalLevels(levels.Count);

        // Adjusted to add currentLevel - 1 to _levelsCompleted
        int levelToMarkAsCompleted = currentLevel - 1;
        if (levelToMarkAsCompleted > 1 && !_gameData._levelsCompleted.Contains(levelToMarkAsCompleted))
        {
            _gameData._levelsCompleted.Add(levelToMarkAsCompleted);
            dataPersistenceManager.SaveGame();
        }
    }
}