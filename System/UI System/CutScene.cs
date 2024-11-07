using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private List<Sprite> _images;
    [SerializeField] private float fadeDuration = 1.5f; // Duration of the fade effect

    // Define a UnityEvent to be fired after the last photo fades out
    public UnityEvent onCutsceneComplete;

    private void Start()
    {
        // Initialize DOTween (if not already done in another part of your project)
        DOTween.Init();

        // Start the cutscene
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Ensure the image is transparent at the start
        if (_image != null)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
        }

        for (int i = 0; i < _images.Count; i++)
        {
            if (_image == null) yield break; // Stop the coroutine if _image is null

            _image.sprite = _images[i]; // Set the current sprite

            // Fade in
            DOTween.Kill(_image); // Kill any previous DOTween animations on _image
            _image.DOFade(1, fadeDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(fadeDuration); // Wait for the fade in to complete

            if (_image == null) yield break; // Check again after the fade in

            // Fade out
            DOTween.Kill(_image); // Kill any previous DOTween animations on _image
            _image.DOFade(0, fadeDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(fadeDuration); // Wait for the fade out to complete

            // If this is the last image, invoke the UnityEvent
            if (i == _images.Count - 1)
            {
                onCutsceneComplete?.Invoke();
            }
        }
    }
}