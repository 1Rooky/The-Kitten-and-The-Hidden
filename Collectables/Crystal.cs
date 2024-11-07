using UnityEngine;

// This class represents a collectable crystal in the game.
public class Crystal : Interactable, IDataPersistence
{
    [SerializeField] private string _crystalId; // The unique identifier for this crystal.

    private bool isCollected = false; // Flag to check if the crystal has been collected.

    // Method to collect the crystal.
    protected virtual void Collect(ObjectType _playerType) => isCollected = true;

    // Override the Interact method from the Interactable base class.
    protected override void Interact(ObjectType _playerType)
    {
        base.Interact(_playerType); // Call the base Interact method.

        Collect(_playerType); // Collect the crystal.
        gameObject.SetActive(false); // Deactivate the crystal game object.
    }

    // Context menu option to generate a unique identifier for the crystal.
    [ContextMenu("Generate guid for ID")]
    private void GenerateID() => _crystalId = System.Guid.NewGuid().ToString();

    // Method to load the game data.
    public void LoadGame(GameData _gameData)
    {
        // Try to get the collected status of the crystal from the game data.
        _gameData._crystalsCollected.TryGetValue(_crystalId, out isCollected);

        // Check if the gameObject reference is still valid before accessing it.
        if (this != null && gameObject != null && isCollected)
        {
            gameObject.SetActive(false);
        }
    }

    // Method to save the game data.
    public void SaveGame(GameData _gameData)
    {
        // If the crystal ID already exists in the game data, update its collected status.
        if (_gameData._crystalsCollected.ContainsKey(_crystalId))
            _gameData._crystalsCollected[_crystalId] = isCollected;
        else
            // If the crystal ID does not exist in the game data, add it.
            _gameData._crystalsCollected.Add(_crystalId, isCollected);
    }
}