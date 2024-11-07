using Febucci.UI.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("Dialog Canvas")]
    [SerializeField] private Canvas _dialogCanvas; // Canvas for displaying dialog UI

    [Header("Dialog Elements")]
    [SerializeField] private Image _iconImage; // Image for the speaker's icon

    [SerializeField] private TextMeshProUGUI _nameText; // Text field for the speaker's name
    [SerializeField] private TextMeshProUGUI _dialogText; // Text field for the dialog content

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource; // Add this line

    [Header("Typewriter")]
    [SerializeField] private TypewriterCore _typewriter; // Typewriter effect for dialog text

    [Header("Dialogs")]
    [SerializeField] private List<DialogText> _dialogs; // List of dialogs to be displayed

    // Event for starting and ending dialog
    internal delegate void DialogEvent(DialogText dialogText);

    internal static event DialogEvent OnDialogStart;

    internal static event DialogEvent OnDialogEnd;

    private InputManager _inputManager; // Manager for handling input actions

    private readonly Queue<string> _dialog = new(); // Queue for managing dialog lines

    private bool _conversationEnded = false; // Flag to check if the conversation has ended
    private int _currentDialogIndex = 0; // Index of the current dialog
    private int _currentCharacterIndex = 0; // Index of the current character in the dialog

    private void OnEnable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>(); // Ensure _inputManager is initialized

        _inputManager.DialogActions.NextDialog.started += HandleNextDialog;
    }

    private void OnDisable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>(); // Ensure _inputManager is initialized

        _inputManager.DialogActions.NextDialog.started -= HandleNextDialog;
    }

    private void Start()
    {
        // Initially hide the dialog canvas if it's not null
        if (_dialogCanvas != null)
            _dialogCanvas.enabled = false;

        _inputManager.DialogActions.Disable(); // Disable dialog actions at start

        foreach (var dialog in _dialogs)
            dialog.IsDialogEnded = false; // Initialize all dialogs as not ended
    }

    public void DisplayNextDialog(int dialogIndex)
    {
        // Enable dialog actions and subscribe to the NextDialog event
        _inputManager.DialogActions.Enable();

        if (_dialog.Count == 0)
        {
            if (!_conversationEnded)
                StartOrContinueDialog(dialogIndex); // Start or continue the dialog if not ended
            else
                EndConversation(); // End the conversation if all dialogs are completed
        }
    }

    private void StartOrContinueDialog(int dialogIndex)
    {
        // Disable unrelated actions during dialog
        _inputManager.CatActions.Disable();
        _inputManager.GhostActions.Disable();

        // Check if the current dialog has ended and proceed accordingly
        if (_dialogs[dialogIndex].IsDialogEnded)
        {
            if (_dialogs[dialogIndex].AfterDialog.Count > 0)
                LoadAfterDialog(dialogIndex);  // Load after dialog if available
            else
                StartConversation(dialogIndex); // Restart the main dialog if no after dialog
        }
        else
            StartConversation(dialogIndex);  // Start the conversation if not ended
    }

    private void StartConversation(int _dialogIndex)
    {
        OnDialogStart?.Invoke(_dialogs[_dialogIndex]); // Broadcast the start event

        _currentDialogIndex = _dialogIndex; // Set the current dialog index

        // Check if _dialogCanvas is not null before trying to access its gameObject
        if (_dialogCanvas != null)
            _dialogCanvas.enabled = true; // Activate the dialog canvas if inactive

        // Set the speaker's icon and name, and enqueue dialog lines
        if (_dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image != null)
        {
            _iconImage.sprite = _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image;
            _iconImage.gameObject.SetActive(true);
        }
        else
            _iconImage.gameObject.SetActive(false);

        _nameText.text = _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._name;

        foreach (string dialog in _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._text)
            _dialog.Enqueue(dialog);

        NextDialog(); // Display the first dialog
    }

    private void LoadAfterDialog(int _dialogIndex)
    {
        if (_dialogs[_dialogIndex].AfterDialog.Count == 0)
        {
            Logging.LogWarning("No after dialog available, consider handling this scenario.");
            return;
        }

        _currentDialogIndex = _dialogIndex; // Set the current dialog index

        if (_dialogCanvas != null)
            _dialogCanvas.enabled = true; // Activate the dialog canvas if inactive

        // Set the speaker's icon and name, and enqueue after dialog lines
        if (_dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image != null)
        {
            _iconImage.sprite = _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image;
            _iconImage.gameObject.SetActive(true);
        }
        else
            _iconImage.gameObject.SetActive(false);

        _nameText.text = _dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._name;

        foreach (string dialog in _dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._text)
            _dialog.Enqueue(dialog);

        NextDialog(); // Display the first after dialog
    }

    private void HandleNextDialog(UnityEngine.InputSystem.InputAction.CallbackContext context) => NextDialog();

    private void NextDialog()
    {
        if (_typewriter.isShowingText)
        {
            _typewriter.SkipTypewriter(); // Skip the typewriter effect if it's currently showing text

            if (_audioSource != null)
                _audioSource.Stop(); // Stop the audio if skipping dialog
        }
        else if (_dialog.Count > 0) // Check if there are more dialog lines in the queue
        {
            string nextDialogText = _dialog.Dequeue(); // Dequeue the next dialog line
            _dialogText.text = nextDialogText; // Set the dialog text
            _typewriter.ShowText(nextDialogText); // Show the text with typewriter effect
        }
        else
        {
            if (_currentCharacterIndex + 1 < GetCurrentDialogList().Count)
            {
                _currentCharacterIndex++; // Move to the next character's dialog
                EnqueueDialogsFromCurrentCharacter(); // Enqueue the next character's dialog lines
            }
            else
                HandleDialogEnd(); // Handle the end of the dialog
        }
    }

    // Helper method to get the current dialog list (main dialog or after dialog)
    private List<Dialog> GetCurrentDialogList()
    {
        if (!_dialogs[_currentDialogIndex].IsDialogEnded)
            return _dialogs[_currentDialogIndex].Dialog;
        else
            return _dialogs[_currentDialogIndex].AfterDialog;
    }

    // Enqueue dialogs from the current character based on the current dialog list
    private void EnqueueDialogsFromCurrentCharacter()
    {
        var currentDialogList = GetCurrentDialogList();
        var currentCharacterDialog = currentDialogList[_currentCharacterIndex];

        _nameText.text = currentCharacterDialog._name;

        if (currentCharacterDialog._image != null)
        {
            _iconImage.sprite = currentCharacterDialog._image;
            _iconImage.gameObject.SetActive(true);
        }
        else
            _iconImage.gameObject.SetActive(false);

        foreach (string dialog in currentCharacterDialog._text)
            _dialog.Enqueue(dialog);

        // Play the audio clip
        if (currentCharacterDialog._audioClip != null)
        {
            _audioSource.clip = currentCharacterDialog._audioClip;
            _audioSource.Play();
        }

        NextDialog(); // Display the next dialog
    }

    private void HandleDialogEnd()
    {
        _dialogs[_currentDialogIndex].IsDialogEnded = true; // Mark the current dialog as ended

        EndConversation(); // Call EndConversation to handle cleanup and state reset
    }

    private void EndConversation()
    {
        OnDialogEnd?.Invoke(_dialogs[_currentDialogIndex]); // Broadcast the end event

        _inputManager.CatActions.Enable(); // Enable cat actions
        _inputManager.GhostActions.Enable(); // Enable ghost actions

        _inputManager.DialogActions.Disable(); // Disable dialog actions

        ResetConversationState(); // Reset the conversation state
    }

    private void ResetConversationState()
    {
        _dialog.Clear(); // Clear the dialog queue
        _iconImage.sprite = null; // Reset the icon image
        _nameText.text = string.Empty; // Clear the name text
        _dialogText.text = string.Empty; // Clear the dialog text
        _conversationEnded = false; // Reset the conversation ended flag
        _currentCharacterIndex = 0; // Reset the current character index

        // Check if _dialogCanvas is not null before trying to access its gameObject
        if (_dialogCanvas != null)
            _dialogCanvas.enabled = false; // Hide the dialog canvas
    }
}