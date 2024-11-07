using UnityEngine;
using UnityEngine.Events;

public class DialogEventsHandler : MonoBehaviour
{
    [SerializeField] private DialogText associatedDialog;
    [SerializeField] private UnityEvent onDialogStart;
    [SerializeField] private UnityEvent onDialogEnd;

    private void OnEnable()
    {
        DialogManager.OnDialogStart += HandleDialogStart;
        DialogManager.OnDialogEnd += HandleDialogEnd;
    }

    private void OnDisable()
    {
        DialogManager.OnDialogStart -= HandleDialogStart;
        DialogManager.OnDialogEnd -= HandleDialogEnd;
    }

    private void HandleDialogStart(DialogText dialogText)
    {
        if (dialogText == associatedDialog)
            onDialogStart.Invoke();
    }

    private void HandleDialogEnd(DialogText dialogText)
    {
        if (dialogText == associatedDialog)
            onDialogEnd.Invoke();
    }
}