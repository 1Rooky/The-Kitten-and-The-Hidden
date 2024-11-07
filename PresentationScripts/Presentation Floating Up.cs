using DG.Tweening;
using UnityEngine;

public class PresentationFloatingUp : MonoBehaviour
{
    public float floatUpDistance = 5f; // The initial distance to float upwards
    public float floatDuration = 5f; // Duration for the initial float up, increased for a more gradual effect
    public float hoverDistance = 0.5f; // Distance to hover up and down after reaching the target
    public float hoverDuration = 1f; // Duration for each hover cycle

    //private void Start()
    //{
    //    // Start the floating up movement when the GameObject is enabled
    //    StartMovement();
    //}

    public void StartMovement()
    {
        // Move the GameObject upwards by floatUpDistance over floatDuration seconds with an easing effect
        transform.DOMoveY(transform.position.y + floatUpDistance, floatDuration)
                 .SetEase(Ease.OutQuad) // Apply an easing function for a more balloon-like ascent
                 .OnComplete(() =>
                 {
                     // After reaching the target, start hovering
                     Hover();
                 });
    }

    private void Hover()
    {
        // Move the GameObject a little up and then down, creating a hovering effect
        // This is achieved by chaining two DOTween animations with SetLoops(-1, LoopType.Yoyo) to make it loop indefinitely
        transform.DOMoveY(transform.position.y + hoverDistance, hoverDuration)
                 .SetEase(Ease.InOutSine) // Apply an easing function for a smoother hover effect
                 .SetLoops(-1, LoopType.Yoyo);
    }
}