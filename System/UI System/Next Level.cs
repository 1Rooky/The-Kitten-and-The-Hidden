using UnityEngine;

public class NextLevel : MonoBehaviour
{
    // Load the next level
    public void LoadLevel(int _level = -1) => ServiceLocator.Instance.GetService<UISystem>().StartLevel(_level);
}