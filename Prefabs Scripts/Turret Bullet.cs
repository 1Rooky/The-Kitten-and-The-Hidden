using DG.Tweening;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    private Tween delayDisableTween;

    private void OnEnable()
    {
        // Cancel any existing delayed call to disable the bullet
        if (delayDisableTween != null && delayDisableTween.IsActive())
        {
            delayDisableTween.Kill();
        }

        // Start a new delayed call to disable the bullet after 5 seconds if it doesn't hit anything
        delayDisableTween = DOVirtual.DelayedCall(5f, () => gameObject.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Disable the bullet immediately upon collision
        gameObject.SetActive(false);

        // Cancel the delayed disable since the bullet has hit something
        if (delayDisableTween.IsActive())
            delayDisableTween.Kill();
    }
}