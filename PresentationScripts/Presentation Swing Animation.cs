using DG.Tweening;
using UnityEngine;

public class PresentationSwingAnimation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f; // Speed of the rotation
    [SerializeField] private float swingAngle = 30.0f; // Angle of the swing
    [SerializeField] private Axis swingAxis = Axis.Z; // Default swing axis

    // Define an enum for the axes
    public enum Axis
    {
        X,
        Y,
        Z
    }

    private void Start() => SwingLogo();

    private void SwingLogo()
    {
        // Determine the rotation axis based on the selected axis
        Vector3 rotationAxis = swingAxis switch
        {
            Axis.X => new Vector3(swingAngle, 0, 0),
            Axis.Y => new Vector3(0, swingAngle, 0),
            Axis.Z => new Vector3(0, 0, swingAngle),
            _ => new Vector3(0, 0, swingAngle),
        };

        // Get the current rotation of the object
        Vector3 currentRotation = transform.eulerAngles;

        // Calculate the target rotations based on the current rotation
        Vector3 targetRotationLeft = currentRotation - rotationAxis;
        Vector3 targetRotationRight = currentRotation + rotationAxis;

        // Adjust the swing duration based on the rotation speed
        float adjustedDuration = rotationSpeed;

        // Create a sequence for the swinging animation
        Sequence swingSequence = DOTween.Sequence();

        // Decide randomly whether to start swinging to the left or to the right
        bool startLeft = Random.value > 0.5f;

        if (startLeft)
        {
            // Start swinging to the left first
            swingSequence.Append(transform.DORotate(targetRotationLeft, adjustedDuration).SetEase(Ease.InOutQuad));
            swingSequence.Append(transform.DORotate(targetRotationRight, adjustedDuration * 2).SetEase(Ease.InOutQuad));
            swingSequence.Append(transform.DORotate(currentRotation, adjustedDuration).SetEase(Ease.InOutQuad));
        }
        else
        {
            // Start swinging to the right first
            swingSequence.Append(transform.DORotate(targetRotationRight, adjustedDuration).SetEase(Ease.InOutQuad));
            swingSequence.Append(transform.DORotate(targetRotationLeft, adjustedDuration * 2).SetEase(Ease.InOutQuad));
            swingSequence.Append(transform.DORotate(currentRotation, adjustedDuration).SetEase(Ease.InOutQuad));
        }

        // Set the sequence to loop indefinitely using Yoyo to ensure smooth transition back and forth
        swingSequence.SetLoops(-1, LoopType.Yoyo);
    }
}