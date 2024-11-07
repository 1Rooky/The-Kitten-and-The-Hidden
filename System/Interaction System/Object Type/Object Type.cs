using UnityEngine;

public class ObjectType : MonoBehaviour
{
    // Serialized fields that can be set in the Unity editor
    [SerializeField] private bool isCat; // Flag to check if the object is a cat

    [SerializeField] private bool isGhost; // Flag to check if the object is a ghost
    [SerializeField] private bool isObject; // Flag to check if the object is an object

    // Properties for the flags
    public bool IsCat { get => isCat; set => isCat = value; } // Property for the isCat flag

    public bool IsGhost { get => isGhost; set => isGhost = value; } // Property for the isGhost flag
    public bool IsObject { get => isObject; set => isObject = value; } // Property for the isObject flag
}