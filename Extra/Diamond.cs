using DG.Tweening;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        // Continuous rotation around the Y-axis
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Restart);

        // Set initial scale to 1.5f to start the sequence
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        // Expand and shrink effect
        Sequence mySequence = DOTween.Sequence();
        // Since we start at 1.3f, we first shrink to 0.7f
        mySequence.Append(transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 3f).SetEase(Ease.InOutQuad));
        // Then expand back to 1.3f
        mySequence.Append(transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 3f).SetEase(Ease.InOutQuad));
        mySequence.SetLoops(-1, LoopType.Yoyo); // Use Yoyo loop type for back and forth animation
    }
}