#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataPersistenceManager))]
public class SaveSystemEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataPersistenceManager myScript = (DataPersistenceManager)target;
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myScript);
            PrefabUtility.ApplyPrefabInstance(myScript.gameObject, InteractionMode.AutomatedAction);
        }
    }
}
#endif