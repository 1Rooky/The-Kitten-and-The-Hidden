using DG.Tweening;
using UnityEngine;

public class PresentationDoTweenAnimation : MonoBehaviour
{
    [Header("Fade Effect")]
    [SerializeField] private float fadeDuration = 1.0f; // Duration of the fade-in animation in seconds

    [Header("Make Object Appear Effect")]
    [SerializeField] private float animationDuration = 2.0f; // Total duration of the appearance animation

    [SerializeField] private Vector3 startScale = new(0.1f, 0.1f, 0.1f); // Starting scale of the object
    [SerializeField] private float moveUpDistance = 1.0f; // Distance the object moves up during the animation

    public void FadeInLogo(GameObject _image)
    {
        // Ensure the GameObject has a CanvasGroup component
        if (!_image.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = _image.AddComponent<CanvasGroup>();
        }

        // Set the initial alpha to 0 to make the image completely transparent
        canvasGroup.alpha = 0;

        // Animate the alpha to 1 over the specified duration to achieve the fade-in effect
        canvasGroup.DOFade(1, fadeDuration);
    }

    public void FadeOutLogo(GameObject _image)
    {
        // Ensure the GameObject has a CanvasGroup component
        if (!_image.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = _image.AddComponent<CanvasGroup>();
        }

        // Set the initial alpha to 1 to make sure the image is fully visible
        canvasGroup.alpha = 1;

        // Animate the alpha to 0 over the specified duration to achieve the fade-out effect
        canvasGroup.DOFade(0, fadeDuration);
    }

    public void MakeObjectAppear(GameObject _object)
    {
        // Ensure the GameObject has a CanvasGroup component for fading
        // This is only necessary for UI elements. For 3D objects, handle material transparency separately.
        if (!_object.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = _object.AddComponent<CanvasGroup>();
        }

        _object.SetActive(true);

        // Set initial states: fully transparent and scaled down
        canvasGroup.alpha = 0;
        _object.transform.localScale = startScale;

        // Create a sequence for the appearance animation
        Sequence appearanceSequence = DOTween.Sequence();

        // Scale the object to its original size
        appearanceSequence.Join(_object.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack));

        // Move the object up
        appearanceSequence.Join(_object.transform.DOMoveY(_object.transform.position.y + moveUpDistance, animationDuration).SetEase(Ease.OutQuad));

        // Fade in the object to full opacity
        appearanceSequence.Join(canvasGroup.DOFade(1, animationDuration));

        // Removed the rotation part to make the object appear from the ground without rotating

        // Set the sequence to play
        appearanceSequence.Play();
    }
}