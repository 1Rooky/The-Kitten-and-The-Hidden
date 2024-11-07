using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Save Slot Config")]
    [SerializeField] private string _profileID;

    [Header("UI Config")]
    [SerializeField] private GameObject _noDataContent;

    [SerializeField] private GameObject _hasDataContent;

    [SerializeField] private TMPro.TextMeshProUGUI _percentageComplete;

    [Header("Clear Button")]
    [SerializeField] private Button _clearButton;

    private Button _saveSlotbutton;

    internal bool HasData { get; private set; } = false;

    private void Awake() => _saveSlotbutton = GetComponent<Button>();

    internal void SetData(GameData gameData)
    {
        if (gameData == null)
        {
            HasData = false;

            _noDataContent.SetActive(true);
            _hasDataContent.SetActive(false);
            _clearButton.interactable = false;

            return;
        }

        HasData = true;

        _noDataContent.SetActive(false);
        _hasDataContent.SetActive(true);
        _clearButton.interactable = true;

        _percentageComplete.text = gameData.PercentageComplete() + "% Complete";
    }

    internal string GetProfileID() => _profileID;

    internal void SetSaveSlotButtonInteractable(bool _interactable)
    {
        _saveSlotbutton.interactable = _interactable;

        if (_saveSlotbutton.TryGetComponent<UIButtonsAnimationAndHoverEffect>(out var interactable))
            interactable.enabled = _interactable;
    }

    internal void SetClearButtonInteractable(bool _interactable)
    {
        _clearButton.interactable = _interactable;

        if (_saveSlotbutton.TryGetComponent<UIButtonsAnimationAndHoverEffect>(out var interactable))
            interactable.enabled = _interactable;
    }
}