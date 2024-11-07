using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDeathCount))]
public class PlayerDeathCountEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerDeathCount myScript = (PlayerDeathCount)target;

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Increase Death Count"))
        {
            myScript.IncreaseDeathCount();
        }

        if (GUILayout.Button("Reset Death Count"))
        {
            myScript.ResetDeathCount();
        }

        if (GUILayout.Button("Decrease Death Count"))
        {
            myScript.DecreaseDeathCount();
        }

        GUILayout.EndHorizontal();
    }
}