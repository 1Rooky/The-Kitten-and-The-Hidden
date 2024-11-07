using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataPersistenceManager))]
public class DataPersistenceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DataPersistenceManager myScript = (DataPersistenceManager)target;

        SerializedProperty iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        while (iterator.NextVisible(false))
            if (iterator.name != "_overwriteSelectedProfile" && iterator.name != "_overwriteSelectedProfileID")
                EditorGUILayout.PropertyField(iterator, true);

        myScript.OverwriteSelectedProfile = EditorGUILayout.Toggle("Overwrite Selected Profile", myScript.OverwriteSelectedProfile);

        if (myScript.OverwriteSelectedProfile)
            myScript.OverwriteSelectedProfileID = EditorGUILayout.TextField("Overwrite Selected Profile ID", myScript.OverwriteSelectedProfileID);

        if (GUI.changed)
            EditorUtility.SetDirty(myScript);

        serializedObject.ApplyModifiedProperties();
    }
}