using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string _name;
    public Sprite _image;
    [TextArea(5, 10)] public List<string> _text;
    public AudioClip _audioClip;
}
