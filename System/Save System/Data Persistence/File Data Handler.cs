using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string _dataDirPath;
    private readonly string _dataFileName;
    private readonly bool _useEncryption;
    private readonly string _encryptionKey = "4 Dcuks <3";
    private readonly string _backupExtension = ".bak";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    public GameData Load(string _profileID, bool _allowRestoreFromBackup = true)
    {
        if (string.IsNullOrEmpty(_profileID))
            return null;

        string _fullPath = Path.Combine(_dataDirPath, _profileID, _dataFileName);

        GameData _loadData = null;

        if (File.Exists(_fullPath))
        {
            try
            {
                using FileStream stream = new(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new(stream);
                string _dataToLoad = reader.ReadToEnd();

                if (_useEncryption)
                    _dataToLoad = EncryptDecrypt(_dataToLoad);

                _loadData = JsonUtility.FromJson<GameData>(_dataToLoad);
            }
            catch (Exception e)
            {
                if (_allowRestoreFromBackup)
                {
                    Logging.LogWarning("Failed to load data file. Attempting to roll back. \n" + e);

                    bool _rollbackSuccess = AttemptRollback(_fullPath);
                    if (_rollbackSuccess)
                        _loadData = Load(_profileID, false);
                }
                else
                {
                    Logging.LogError("Error loading file: " + _fullPath + " - " + e.Message);
                }
            }
        }

        return _loadData;
    }

    public void Save(GameData gameData, string _profileID)
    {
        if (string.IsNullOrEmpty(_profileID))
            return;

        string _fullPath = Path.Combine(_dataDirPath, _profileID, _dataFileName);
        string _backupPath = _fullPath + _backupExtension;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));

            string _dataToStore = JsonUtility.ToJson(gameData, true);

            if (_useEncryption)
                _dataToStore = EncryptDecrypt(_dataToStore);

            using FileStream stream = new(_fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using StreamWriter writer = new(stream);
            writer.Write(_dataToStore);
            writer.Flush();

            GameData _verifiedGameData = Load(_profileID);
            if (_verifiedGameData != null)
                File.Copy(_fullPath, _backupPath, true);
            else
                throw new Exception("Error saving file: " + _fullPath + " - Data verification failed.");
        }
        catch (Exception e)
        {
            Logging.LogError("Error saving file: " + _fullPath + " - " + e.Message);
        }
    }

    public void Delete(string _profileID)
    {
        if (string.IsNullOrEmpty(_profileID))
            return;

        string _fullPath = Path.Combine(_dataDirPath, _profileID, _dataFileName);

        try
        {
            if (File.Exists(_fullPath))
                Directory.Delete(Path.GetDirectoryName(_fullPath), true);
            else
                Logging.LogWarning("No save data found for profile: " + _profileID);
        }
        catch (Exception e)
        {
            Logging.LogError("Error deleting file: " + _fullPath + " - " + e.Message);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string _modifiedData = string.Empty;

        for (int i = 0; i < data.Length; i++)
            _modifiedData += (char)(data[i] ^ _encryptionKey[i % _encryptionKey.Length]);

        return _modifiedData;
    }

    public Dictionary<string, GameData> LoadAllProfile()
    {
        Dictionary<string, GameData> _allProfileData = new();

        IEnumerable<DirectoryInfo> _profileIDs = new DirectoryInfo(_dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo _profileID in _profileIDs)
        {
            string _profileName = _profileID.Name;
            string _fullPath = Path.Combine(_dataDirPath, _profileName, _dataFileName);

            if (!File.Exists(_fullPath))
            {
                Logging.LogWarning("No save data found for profile: " + _profileName);
                continue;
            }

            GameData _loadData = Load(_profileName);

            if (_loadData != null)
                _allProfileData.Add(_profileName, _loadData);
            else
                Logging.LogError("Error loading save data for profile: " + _profileName);
        }

        return _allProfileData;
    }

    public string GetMostRecentlyUpdatedProfile()
    {
        string _mostRecentlyUpdatedProfile = string.Empty;

        Dictionary<string, GameData> _allProfileData = LoadAllProfile();

        foreach (KeyValuePair<string, GameData> _profileData in _allProfileData)
        {
            string _profileID = _profileData.Key;
            GameData _gameData = _profileData.Value;

            if (_gameData == null)
                continue;

            if (string.IsNullOrEmpty(_mostRecentlyUpdatedProfile))
                _mostRecentlyUpdatedProfile = _profileID;
            else
            {
                DateTime _mostRecentlyUpdated = DateTime.FromBinary(_allProfileData[_mostRecentlyUpdatedProfile]._lastUpdated);
                DateTime _newProfileUpdated = DateTime.FromBinary(_gameData._lastUpdated);

                if (_newProfileUpdated > _mostRecentlyUpdated)
                    _mostRecentlyUpdatedProfile = _profileID;
            }
        }

        return _mostRecentlyUpdatedProfile;
    }

    private bool AttemptRollback(string _fullPath)
    {
        bool _scucess = false;

        string _backupPath = _fullPath + _backupExtension;

        try
        {
            if (File.Exists(_backupPath))
            {
                File.Copy(_backupPath, _fullPath, true);
                _scucess = true;
                Logging.LogWarning("Rollback successful for file: " + _fullPath);
            }
            else
            {
                throw new Exception("No backup found for file: " + _fullPath);
            }
        }
        catch (Exception e)
        {
            Logging.LogError("Error rolling back file: " + _fullPath + " - " + e.Message);
        }

        return _scucess;
    }
}