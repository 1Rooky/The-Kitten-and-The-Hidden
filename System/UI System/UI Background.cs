using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackground : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private List<Sprite> _backgroundImages;
    [SerializeField] private float transitionDuration = 1.5f; // Duration for fade in/out
    [SerializeField] private float delayBetweenTransitions = 2f; // Delay between transitions

    private WaitForSeconds waitForSeconds;
    private Sequence movementSequence; // Store the sequence for later access

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(transitionDuration * 2 + delayBetweenTransitions);

        if (_backgroundImages.Count > 0)
            StartCoroutine(BackgroundTransitionCoroutine());
    }

    private IEnumerator BackgroundTransitionCoroutine()
    {
        while (true)
        {
            if (_background == null) yield break; // Stop the coroutine if _background is null

            // Select a random image from the list
            Sprite nextImage = _backgroundImages[Random.Range(0, _backgroundImages.Count)];

            // Fade out
            yield return _background.DOFade(0f, transitionDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

            if (_background == null) yield break; // Check again after the fade out

            // Change the image when fully faded out
            _background.sprite = nextImage;

            // Reset scale and position to default before applying transformations
            _background.rectTransform.localScale = Vector3.one;
            _background.rectTransform.anchoredPosition = Vector2.zero;

            // Apply random zoom
            float zoomFactor = Random.Range(1.1f, 1.5f); // Random zoom between 110% to 150%
            _background.rectTransform.DOScale(zoomFactor, transitionDuration);

            // Apply continuous movement
            float moveDistance = 100f; // Adjust this value based on your UI layout
            Vector2 startPosition = Random.Range(0, 2) == 0 ? new Vector2(-moveDistance, 0) : new Vector2(moveDistance, 0);
            Vector2 endPosition = startPosition.x > 0 ? new Vector2(-moveDistance, 0) : new Vector2(moveDistance, 0);
            _background.rectTransform.anchoredPosition = startPosition;

            // Use a single duration for both directions
            float movementDuration = transitionDuration * 6f;

            // Create a looping movement sequence with the unified duration
            movementSequence = DOTween.Sequence()
                .Append(_background.rectTransform.DOAnchorPos(endPosition, movementDuration).SetEase(Ease.InOutQuad))
                .Append(_background.rectTransform.DOAnchorPos(startPosition, movementDuration).SetEase(Ease.InOutQuad))
                .SetLoops(-1, LoopType.Yoyo); // Loop indefinitely, alternating directions

            // Fade in
            _background.DOFade(1f, transitionDuration).SetEase(Ease.InOutQuad);

            // Wait for the transition to complete and the delay before starting the next transition
            yield return waitForSeconds;
        }
    }

    private void OnDestroy()
    {
        // Ensure to kill the movement sequence to stop it before destroying the object
        movementSequence?.Kill();

        // Stop all coroutines to prevent errors when the object is destroyed
        StopAllCoroutines();
    }
}