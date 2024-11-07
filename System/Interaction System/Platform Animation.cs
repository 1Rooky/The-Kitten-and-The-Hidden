using DG.Tweening;
using UnityEngine;

public class PlatformAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 startPositionOffset = Vector3.zero; // Offset from the current position to start
    [SerializeField] private Vector3 endPositionOffset = Vector3.up * 5f; // Offset from the current position to end
    [SerializeField] private float moveDuration = 2f; // Duration of the movement
    [SerializeField] private Ease _easeStyle = Ease.OutQuad; // Duration of the movement

    public void StartMovementAnimation()
    {
        // Calculate the absolute start and end positions based on the current position and specified offsets
        Vector3 absoluteStartPosition = transform.position + startPositionOffset;
        Vector3 absoluteEndPosition = transform.position + endPositionOffset;

        // Move the platform to the start position instantly (without animation)
        transform.position = absoluteStartPosition;

        // Animate the platform's position to the end position over the specified duration
        transform.DOMove(absoluteEndPosition, moveDuration).SetEase(_easeStyle);
    }
}