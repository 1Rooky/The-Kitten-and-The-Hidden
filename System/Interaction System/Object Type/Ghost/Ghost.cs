using UnityEngine;

// Ghost class inherits from MonoBehaviour
public class Ghost : MonoBehaviour
{
    // Called when the object is first initialized
    private void Awake() => ServiceLocator.Instance.RegisterService(this, false); // Register this service in the ServiceLocator

    // Method to get the Transform component of the ghost
    internal Transform GetTransform() => transform;
}
