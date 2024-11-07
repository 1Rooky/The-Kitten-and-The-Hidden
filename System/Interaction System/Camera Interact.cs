using UnityEngine;

public class CameraInteract : Interactable
{
    protected override void Interact(ObjectType _objectType)
    {
        if (_objectType.IsCat)
            base.Interact(_objectType);
    }
}