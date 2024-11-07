using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private float _timer = 5f;

    private WaitForSeconds _wait;

    private void Start() => _wait = new WaitForSeconds(_timer);

    private void OnEnable() => StartCoroutine(DeactivateAfterDelay());

    private IEnumerator DeactivateAfterDelay()
    {
        yield return _wait;

        //ObjectPool.SharedInstance.ReturnToPool(2, gameObject);
    }
}