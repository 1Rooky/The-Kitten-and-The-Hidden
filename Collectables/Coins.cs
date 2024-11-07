using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string _coinID;

    private bool isCollected = false;

    [ContextMenu("Generate guid for ID")]
    private void GenerateID()
    {
        _coinID = System.Guid.NewGuid().ToString();
    }

    public void CollectCoin()
    {
        isCollected = true;
        gameObject.SetActive(false);
    }

    public void LoadGame(GameData _gameData)
    {
        _gameData._coinsCollected.TryGetValue(_coinID, out isCollected);

        if (isCollected)
            gameObject.SetActive(false);
    }

    public void SaveGame(GameData _gameData)
    {
        if (_gameData._coinsCollected.ContainsKey(_coinID))
        {
            _gameData._coinsCollected[_coinID] = isCollected;
        }
        else
        {
            _gameData._coinsCollected.Add(_coinID, isCollected);
        }
    }
}