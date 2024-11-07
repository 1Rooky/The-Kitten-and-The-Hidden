using UnityEngine;

// This script requires a SphereCollider component on the same GameObject
[RequireComponent(typeof(SphereCollider))]
public class EnemyLineChecker : MonoBehaviour
{
    // Serialized field for the SphereCollider component
    [SerializeField] private SphereCollider _collider;

    // Field of view for the enemy
    private float _fieldOfView = 90f;

    // LayerMask to determine what layers the enemy can see
    private LayerMask _lineOfSightLayers;

    // Reference to the cat in the trigger area
    private Cat _catInTrigger; // Use Object Type Script

    // Delegate for the event when the enemy gains sight of the cat
    public delegate void GainSightEvent(Cat _cat);

    // Delegate for the event when the enemy loses sight of the cat
    public delegate void LoseSightEvent(Cat _cat);

    // Event triggered when the enemy gains sight of the cat
    public event GainSightEvent OnGainSight;

    // Event triggered when the enemy loses sight of the cat
    public event LoseSightEvent OnLoseSight;

    // Property for the field of view
    internal float FieldOfView { get => _fieldOfView; set => _fieldOfView = value; }

    // Property for the radius of the collider
    internal float ColliderRadius { get => _collider.radius; set => _collider.radius = value; }

    // Property for the line of sight layers
    internal LayerMask LineOfSightLayers { get => _lineOfSightLayers; set => _lineOfSightLayers = value; }

    // Called when the object is first initialized
    private void Awake() => _collider = GetComponent<SphereCollider>();

    // Called every fixed framerate frame
    private void FixedUpdate()
    {
        // If there is a cat in the trigger area, check the line of sight
        if (_catInTrigger != null)
            CheckLineOfSight(_catInTrigger);
    }

    // Called when the object enters a trigger
    private void OnTriggerEnter(Collider other)
    {
        // If the other object is a cat, set it as the cat in the trigger area
        if (other.TryGetComponent(out Cat cat))
            _catInTrigger = cat;
    }

    // Called when the object exits a trigger
    private void OnTriggerExit(Collider other)
    {
        // If the other object is a cat, invoke the OnLoseSight event and set the cat in the trigger area to null
        if (other.TryGetComponent(out Cat cat))
        {
            OnLoseSight?.Invoke(cat);
            _catInTrigger = null;
        }
    }

    // Check the line of sight to the cat
    private bool CheckLineOfSight(Cat cat)
    {
        // Calculate the direction to the cat
        Vector3 _direction = (cat.transform.position - transform.position).normalized;

        // If the cat is within the field of view
        if (Vector3.Dot(transform.forward, _direction) >= Mathf.Cos(_fieldOfView))
            // If there is no obstacle between the enemy and the cat
            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, _collider.radius, _lineOfSightLayers))
                // If the hit object is a cat
                if (hit.transform.GetComponent<Cat>() != null)
                {
                    // Invoke the OnGainSight event and return true
                    OnGainSight?.Invoke(cat);
                    return true;
                }

        // If the cat is not within the field of view or there is an obstacle, return false
        return false;
    }
}