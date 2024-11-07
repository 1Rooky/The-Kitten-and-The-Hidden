using UnityEditor;

[CustomEditor(typeof(Possess))]
public class PossessManagerEditor : InteractableEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Possess manager = (Possess)target;
        SerializedObject so = new(manager);

        SerializedProperty _possessableScriptProperty = so.FindProperty("_possessableScript");
        EditorGUILayout.PropertyField(_possessableScriptProperty);

        if (_possessableScriptProperty.objectReferenceValue is not IPossessable)
        {
            EditorGUILayout.HelpBox("The assigned script does not implement the IPossessable interface!", MessageType.Error);
            _possessableScriptProperty.objectReferenceValue = null;
        }

        so.ApplyModifiedProperties();
    }
}