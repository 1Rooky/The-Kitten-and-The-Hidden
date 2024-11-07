using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long _lastUpdated;

    public int _deathCount;
    public Vector3 _playerPosition;
    public SerializableDictionary<string, bool> _coinsCollected;
    public SerializableDictionary<string, bool> _crystalsCollected;

    public SerializableDictionary<string, bool> _checkPointPassed;
    public string _lastCheckPointId;

    public List<int> _levelsCompleted;
    public int _totalLevelsMinusOne;

    public GameData()
    {
        _deathCount = 0;
        _playerPosition = Vector3.zero;
        _coinsCollected = new SerializableDictionary<string, bool>();
        _crystalsCollected = new SerializableDictionary<string, bool>();

        _checkPointPassed = new SerializableDictionary<string, bool>();
        _lastCheckPointId = string.Empty;

        _levelsCompleted = new();
        _totalLevelsMinusOne = 0;
    }

    /*    public int PercentageComplete()
        {
            int _totalcoinsCollected = 0;

            foreach (bool coinCollected in _coinsCollected.Values)
            {
                if (coinCollected)
                    _totalcoinsCollected++;
            }

            int _precentageComplete = 0;

            if (_coinsCollected.Count > 0)
            {
                _precentageComplete = (_totalcoinsCollected * 100 / _coinsCollected.Count);
            }

            return _precentageComplete;
        }*/

    public void SetTotalLevels(int totalLevels) => _totalLevelsMinusOne = totalLevels - 1;

    public int PercentageComplete()
    {
        //Logging.Log($"Levels Completed: {_levelsCompleted.Count}, Total Levels Minus One: {_totalLevelsMinusOne}");

        int levelsCompleted = _levelsCompleted.Count;
        int _percentageComplete = 0;

        if (_totalLevelsMinusOne > 0)
        {
            _percentageComplete = (levelsCompleted * 100) / _totalLevelsMinusOne;
        }

        return _percentageComplete;
    }
}