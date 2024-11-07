using Unity.Cinemachine;
using UnityEngine;

public class TargetGroup : MonoBehaviour
{
    [SerializeField] private float _weight = 1.0f; // The weight of each target in the target group
    [SerializeField] private float _radius = 0.5f; // The radius of each target in the target group

    private CinemachineTargetGroup _targetGroup; // Reference to the CinemachineTargetGroup component
    private ServiceLocator _serviceLocator; // Reference to the ServiceLocator

    private void Awake()
    {
        // Initialize the ServiceLocator and register this component as a service
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, false);
    }

    private void Start()
    {
        SetTargetGroup();
    }

    // Sets up the target group by adding specific targets to it.
    internal void SetTargetGroup()
    {
        // Attempt to get the CinemachineTargetGroup component attached to this GameObject
        if (TryGetComponent(out _targetGroup))
        {
            // Clear any existing targets in the target group
            _targetGroup.Targets.Clear();

            // Add the Cat as a target to the target group
            _targetGroup.Targets.Add(new CinemachineTargetGroup.Target
            {
                Object = _serviceLocator.GetService<Cat>().GetTransform(), // Get the Cat's transform
                Weight = _weight, // Set the weight for the Cat
                Radius = _radius // Set the radius for the Cat
            });

            // Add the Ghost as a target to the target group
            _targetGroup.Targets.Add(new CinemachineTargetGroup.Target
            {
                Object = _serviceLocator.GetService<Ghost>().GetTransform(), // Get the Ghost's transform
                Weight = _weight, // Set the weight for the Ghost
                Radius = _radius // Set the radius for the Ghost
            });
        }
        else
            // Log a message if the CinemachineTargetGroup component is not found
            Logging.Log("haai i am null, NO");
    }
}