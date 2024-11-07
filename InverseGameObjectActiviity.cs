using UnityEngine;

public class InverseGameObjectActiviity : MonoBehaviour
{
    public void InverseActivity(GameObject obj) => obj.SetActive(!obj.activeSelf);
}