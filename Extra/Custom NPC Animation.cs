using DG.Tweening;
using UnityEngine;

public class CustomNPCAnimation : MonoBehaviour
{
    public float floatDistance = 0.5f; // The distance the NPC will float up and down
    public float floatDuration = 2f; // The duration of one float cycle (up and down)
    private Tweener floatTweener; // Reference to the active float tweener

    private void Start() => StartIdleFloating(); // Start the idle floating animation

    private void OnDisable() => floatTweener?.Kill(); // Safely kill the float tweener if it exists

    private void OnDestroy() => floatTweener?.Kill(); // Safely kill the float tweener if it exists

    private void StartIdleFloating()
    {
        // Check if the transform component is still available
        if (transform != null)
        {
            // Move the NPC up and down
            floatTweener = transform.DOMoveY(transform.position.y + floatDistance, floatDuration)
                .SetEase(Ease.InOutSine) // Smooth start and end
                .SetLoops(-1, LoopType.Yoyo); // Loop the animation back and forth infinitely
        }
    }
}