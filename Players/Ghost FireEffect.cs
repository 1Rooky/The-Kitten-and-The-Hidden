using UnityEngine;
using UnityEngine.VFX;

public class GhostFireEffect : MonoBehaviour
{
    [SerializeField] private VisualEffect ghostFireVFX; // Assign this in the inspector
    [SerializeField] private float newPlayRate = 0.5f; // Example: Set this to 0.5 to play the VFX at half speed

    private void Start()
    {
        if (ghostFireVFX != null)
            ghostFireVFX.playRate = newPlayRate;
        else
            Logging.LogWarning("VisualEffect component not assigned to GhostFireEffect script.");
    }
}