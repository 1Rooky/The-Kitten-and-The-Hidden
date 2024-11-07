public interface IDataPersistence
{
    void LoadGame(GameData _gameData);

    void SaveGame(GameData _gameData);
}