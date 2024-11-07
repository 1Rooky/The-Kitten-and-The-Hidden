using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPawnAnimation : MonoBehaviour
{
    [SerializeField] private List<Image> footprints; // Use Image components instead
    public float fadeDuration = 1f; // Duration of each fade in/out
    public float delayBetweenFootprints = 0.5f; // Delay between each footprint animation

    private WaitForSeconds waitForSeconds;

    private void Start()
    {
        SetFootprintsInvisible(); // Make all footprints invisible at the start
        waitForSeconds = new WaitForSeconds(fadeDuration + delayBetweenFootprints);
        StartCoroutine(AnimateFootprints());
    }

    private void SetFootprintsInvisible()
    {
        foreach (var footprint in footprints)
        {
            if (footprint != null) // Check if the footprint is not null
                footprint.color = new Color(footprint.color.r, footprint.color.g, footprint.color.b, 0);
        }
    }

    private IEnumerator AnimateFootprints()
    {
        while (true) // Loop indefinitely
        {
            for (int i = 0; i < footprints.Count; i++)
            {
                if (footprints[i] == null) continue; // Skip if the current footprint is null

                // Fade in the current footprint
                footprints[i].DOFade(1f, fadeDuration).SetEase(Ease.Linear);

                // If not the first footprint, start fading out the previous footprint
                if (i > 0 && footprints[i - 1] != null)
                {
                    footprints[i - 1].DOFade(0f, fadeDuration).SetEase(Ease.Linear);
                }

                // Wait for the fade in to complete plus the delay
                yield return waitForSeconds;

                // Special handling for the last footprint to ensure smooth looping
                if (i == footprints.Count - 1)
                {
                    if (footprints[i] != null)
                    {
                        // Start fading out the last footprint
                        footprints[i].DOFade(0f, fadeDuration).SetEase(Ease.Linear);
                    }

                    if (footprints[0] != null)
                    {
                        // Ensure the first footprint starts fading in before the last one fades out completely
                        footprints[0].DOFade(1f, fadeDuration).SetEase(Ease.Linear);
                    }

                    // Wait for the last fade out and the first fade in to complete before restarting the loop
                    yield return waitForSeconds;
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Ensure to kill all DOTween animations to stop them before destroying the object
        foreach (var footprint in footprints)
        {
            if (footprint != null) footprint.DOKill();
        }
    }
}