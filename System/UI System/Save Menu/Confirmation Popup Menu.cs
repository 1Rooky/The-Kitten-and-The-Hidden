using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _displayText;

    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    public void ActivateMenu(string _displayText, UnityAction _confirmAction, UnityAction _cancelAction)
    {
        gameObject.SetActive(true);

        this._displayText.text = _displayText;

        _confirmButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _confirmButton.onClick.AddListener(() =>
        {
            DecativeMenu();
            _confirmAction();
        });

        _cancelButton.onClick.AddListener(() =>
        {
            DecativeMenu();
            _cancelAction();
        });
    }

    private void DecativeMenu() => gameObject.SetActive(false);
}