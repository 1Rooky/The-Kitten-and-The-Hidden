using DG.Tweening;
using UnityEngine;

public class CustomCloudsAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float scaleMultiplier = 1.2f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Sequence cloudSequence;

    private void OnEnable()
    {
        // Check if the transform component is still available
        if (transform != null)
        {
            originalPosition = transform.position;
            originalScale = transform.localScale;

            StartCloudAnimation();
        }
    }

    private void OnDisable() => cloudSequence?.Kill(); // Safely kill the DOTween sequence if it exists

    private void OnDestroy() => cloudSequence?.Kill(); // Safely kill the DOTween sequence if it exists

    private void StartCloudAnimation()
    {
        // Check if the GameObject has been destroyed
        if (this == null || gameObject == null) return;

        // Reset the cloud's position and scale
        transform.position = originalPosition;
        transform.localScale = originalScale;

        cloudSequence = DOTween.Sequence();

        // Append animations to the sequence
        // Note: No need to check for null in each step as the sequence handling is encapsulated
        cloudSequence.Append(transform.DOMoveX(originalPosition.x + moveDistance, duration).SetEase(Ease.InOutQuad));
        cloudSequence.Append(transform.DOMoveX(originalPosition.x - moveDistance, duration * 2).SetEase(Ease.InOutQuad));
        cloudSequence.Append(transform.DOMoveX(originalPosition.x, duration).SetEase(Ease.InOutQuad));

        cloudSequence.Join(transform.DOScale(originalScale * scaleMultiplier, duration).SetEase(Ease.InOutQuad));
        cloudSequence.Join(transform.DOScale(originalScale, duration * 2).SetEase(Ease.InOutQuad));

        cloudSequence.Join(transform.DORotate(new Vector3(0, 0, 10), duration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad));
        cloudSequence.Join(transform.DORotate(Vector3.zero, duration * 2, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad));

        cloudSequence.SetLoops(-1, LoopType.Restart);

        cloudSequence.Play();
    }
}