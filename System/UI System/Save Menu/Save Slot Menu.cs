using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotMenu : MonoBehaviour
{
    [SerializeField] private ConfirmationPopupMenu _confirmationPopupMenu;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject _gameObject;

    private SaveSlot[] _saveSlot;

    private ServiceLocator _serviceLocator;
    private DataPersistenceManager _dataPersistenceManager;
    private SceneManagement _sceneManagement;

    private bool _isGameLoading = false;

    private void Awake()
    {
        _saveSlot = GetComponentsInChildren<SaveSlot>();
        _serviceLocator = ServiceLocator.Instance;

        _dataPersistenceManager = _serviceLocator.GetService<DataPersistenceManager>();
        _sceneManagement = _serviceLocator.GetService<SceneManagement>();
    }

    private void ActivatedMenu(bool _isGameLoading)
    {
        this._isGameLoading = _isGameLoading;

        Dictionary<string, GameData> _profilesGameData = _dataPersistenceManager.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlot)
        {
            _profilesGameData.TryGetValue(saveSlot.GetProfileID(), out GameData _profileData);
            saveSlot.SetData(_profileData);

            if (_profileData == null && _isGameLoading)
            {
                saveSlot.SetSaveSlotButtonInteractable(false);
                saveSlot.SetClearButtonInteractable(false);
            }
            else
            {
                saveSlot.SetSaveSlotButtonInteractable(true);
                saveSlot.SetClearButtonInteractable(true);
            }
        }
    }

    public void UpdateClearButtonInteractability()
    {
        Dictionary<string, GameData> _profilesGameData = _dataPersistenceManager.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlot)
        {
            _profilesGameData.TryGetValue(saveSlot.GetProfileID(), out GameData _profileData);
            saveSlot.SetData(_profileData);

            if (_profileData == null)
            {
                saveSlot.SetClearButtonInteractable(false);
            }
            else
            {
                saveSlot.SetClearButtonInteractable(true);
            }
        }
    }

    public void OnSaveSlotClicked(SaveSlot _saveslot)
    {
        DisableMenuButtons();

        if (_isGameLoading)
        {
            _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());

            // Retrieve the GameData for the selected profile
            GameData gameData = _dataPersistenceManager.GetCurrentGameData();

            if (gameData != null && _sceneManagement != null)
            {
                // Set the currentLevel to the next level after the last completed one
                int nextLevel = gameData._levelsCompleted.Count > 0 ? gameData._levelsCompleted.Max() : 1;

                _sceneManagement.CurrentLevel = nextLevel;
            }

            SaveGameAndLoadScene();
        }
        else if (_saveslot.HasData)
        {
            _confirmationPopupMenu.ActivateMenu(
                "Do you want to overwrite this save?",
                () =>
                {
                    _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());
                    _dataPersistenceManager.NewGame();
                    SaveGameAndLoadScene();
                },
                () => ActivatedMenu(_isGameLoading));

            if (eventSystem != null)
                eventSystem.firstSelectedGameObject = _gameObject;
        }
        else
        {
            _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());
            _dataPersistenceManager.NewGame();
            SaveGameAndLoadScene();
        }
    }

    public void OnClearButtonClicked(SaveSlot _saveslot)
    {
        DisableMenuButtons();

        _confirmationPopupMenu.ActivateMenu(
            "Do you want to clear this save?",
            () =>
            {
                _dataPersistenceManager.DeleteProfileData(_saveslot.GetProfileID());
                ActivatedMenu(_isGameLoading);
            },
            () => ActivatedMenu(_isGameLoading)
            );

        if (eventSystem != null)
            eventSystem.firstSelectedGameObject = _gameObject;
    }

    private void SaveGameAndLoadScene()
    {
        _dataPersistenceManager.SaveGame();
        _sceneManagement.StartLevel();
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in _saveSlot)
        {
            saveSlot.SetSaveSlotButtonInteractable(false);
            saveSlot.SetClearButtonInteractable(false);
        }
    }
}