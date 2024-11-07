using UnityEngine;

public class DontDestoryOnLoadObjects : MonoBehaviour
{
    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
}