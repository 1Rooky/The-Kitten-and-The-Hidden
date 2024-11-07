using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Data Storage Config")]
    [SerializeField] private string _fileName;

    [SerializeField] private bool _useEncryption;

    [Header("Auto Save Config")]
    [SerializeField] private Canvas _autosaveCanvas;

    [SerializeField] private Image _autoSaveImage;
    [SerializeField] private float _autoSaveTimeInSeconds = 60f;

    [Header("For Debugging")]
    [SerializeField] private bool _disableDataPersistence = false;

    [SerializeField] private bool _initializeDataIfNull = false;
    [SerializeField] private bool _overwriteSelectedProfile = false;
    [SerializeField] private string _overwriteSelectedProfileID;

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _fileDataHandler;
    private ServiceLocator _serviceLocator;

    private string _selectedProfileID = string.Empty;
    private Coroutine _autoSaveCoroutine;

    public bool OverwriteSelectedProfile { get => _overwriteSelectedProfile; set => _overwriteSelectedProfile = value; }
    public string OverwriteSelectedProfileID { get => _overwriteSelectedProfileID; set => _overwriteSelectedProfileID = value; }

    #region Unity Functions

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;

        _serviceLocator.RegisterService(this, true);
        _dataPersistenceObjects = new();

        if (_disableDataPersistence)
            Logging.LogWarning("Data Persistence is disabled.");

        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);

        InitializeSelectedProfileID();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnApplicationQuit() => SaveGame();

    #endregion Unity Functions

    #region Save and Load System

    public void NewGame() => _gameData = new GameData();

    private void LoadGame()
    {
        if (_disableDataPersistence)
            return;

        _gameData = _fileDataHandler.Load(_selectedProfileID);

        if (_gameData == null && _initializeDataIfNull)
            NewGame();

        if (_gameData == null)
        {
            Logging.LogWarning("No save data found.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
            dataPersistenceObject.LoadGame(_gameData);
    }

    public void SaveGame()
    {
        if (_disableDataPersistence)
            return;

        if (_gameData == null)
        {
            Logging.LogWarning("No save data found.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
            dataPersistenceObject.SaveGame(_gameData);

        _gameData._lastUpdated = System.DateTime.Now.ToBinary();

        _fileDataHandler.Save(_gameData, _selectedProfileID);
    }

    #endregion Save and Load System

    #region Functions

    public GameData GetCurrentGameData() => _gameData;

    internal bool HasGameStarted() => _gameData != null;

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        List<IDataPersistence> dataPersistenceObjects = new();

        // Check regular scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                IDataPersistence[] sceneDataPersistenceObjects = rootObject.GetComponentsInChildren<IDataPersistence>();
                dataPersistenceObjects.AddRange(sceneDataPersistenceObjects);
            }
        }

        var services = _serviceLocator.GetDontDestroyOnLoadServices();
        foreach (var service in services)
        {
            if (service is IDataPersistence dataPersistenceService)
            {
                dataPersistenceObjects.Add(dataPersistenceService);
            }
        }

        return dataPersistenceObjects;
    }

    internal Dictionary<string, GameData> GetAllProfilesGameData() => _fileDataHandler.LoadAllProfile();

    internal void ChangeSelectedProfile(string _newProfileID)
    {
        _selectedProfileID = _newProfileID;
        LoadGame();
    }

    internal void DeleteProfileData(string _profileID)
    {
        _fileDataHandler.Delete(_profileID);
        InitializeSelectedProfileID();
        LoadGame();
    }

    private void InitializeSelectedProfileID()
    {
        _selectedProfileID = _fileDataHandler.GetMostRecentlyUpdatedProfile();

        if (_overwriteSelectedProfile)
        {
            _selectedProfileID = _overwriteSelectedProfileID;
            Logging.LogWarning("Overwriting selected profile with: " + _selectedProfileID);
        }
    }

    #region Auto Save

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(_autoSaveTimeInSeconds);

            // Start autosave visual feedback
            StartCoroutine(AutoSaveFeedback());

            SaveGame();
            Logging.Log("Auto Save The Game");
        }
    }

    private IEnumerator AutoSaveFeedback()
    {
        _autosaveCanvas.enabled = true;

        float duration = 5.0f; // Duration of one cycle (fade in and fade out)
        float time = 0;

        while (time < duration)
        {
            // Oscillate alpha between 150/255 and 200/255
            float alpha = Mathf.Lerp(150f / 255f, 200f / 255f, Mathf.PingPong(time * 10 / duration, 1));
            _autoSaveImage.color = new Color(_autoSaveImage.color.r, _autoSaveImage.color.g, _autoSaveImage.color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        _autosaveCanvas.enabled = false;
    }

    #endregion Auto Save

    #region Scene Management

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        // Find all IDataPersistence objects in the new scene
        List<IDataPersistence> newDataPersistenceObjects = FindAllDataPersistenceObjects();

        // Merge with existing _dataPersistenceObjects to ensure persistent objects are not lost
        foreach (var obj in newDataPersistenceObjects)
        {
            if (!_dataPersistenceObjects.Contains(obj))
            {
                _dataPersistenceObjects.Add(obj);
            }
        }

        LoadGame();

        if (_autoSaveCoroutine != null)
            StopCoroutine(_autoSaveCoroutine);

        _autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    #endregion Scene Management

    #endregion Functions
}