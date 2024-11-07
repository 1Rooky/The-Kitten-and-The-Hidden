using UnityEngine;

public class PlayerDeathCount : MonoBehaviour, IDataPersistence
{
    [Header("Death Count")]
    [SerializeField] private int _deathCount = 0;

    private UISystem _uiSystem;

    private void Start()
    {
        _uiSystem = ServiceLocator.Instance.GetService<UISystem>();
        _uiSystem._deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void IncreaseDeathCount()
    {
        _deathCount++;
        _uiSystem._deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void DecreaseDeathCount()
    {
        _deathCount--;
        _uiSystem._deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void ResetDeathCount()
    {
        _deathCount = 0;
        _uiSystem._deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void LoadGame(GameData _gameData)
    {
        _deathCount = _gameData._deathCount;

        _uiSystem = ServiceLocator.Instance.GetService<UISystem>();
        _uiSystem._deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData._deathCount = _deathCount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ServiceLocator.Instance.GetService<SceneManagement>().StartLevel(1);
        }
    }
}