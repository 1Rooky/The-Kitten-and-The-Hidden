using UnityEngine;

// Cat class inherits from MonoBehaviour
public class Cat : MonoBehaviour
{
    [SerializeField] private Animator _animator; // Reference to the Animator component

    // Called when the object is first initialized
    private void Awake() => ServiceLocator.Instance.RegisterService(this, false); // Register this service in the ServiceLocator

    // Method to get the Transform component of the cat
    public Transform GetTransform() => transform;
}