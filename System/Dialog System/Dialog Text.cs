using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog Text", menuName = "Dialog System/Dialog Text")]
public class DialogText : ScriptableObject
{
    [SerializeField] private List<Dialog> _dialog;
    [SerializeField] private List<Dialog> _afterDialog;

    private bool _isDialogEnded = false;

    public List<Dialog> Dialog => _dialog;
    public List<Dialog> AfterDialog => _afterDialog;
    public bool IsDialogEnded { get => _isDialogEnded; set => _isDialogEnded = value; }
}