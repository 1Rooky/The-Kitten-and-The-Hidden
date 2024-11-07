using UnityEditor;

[CustomEditor(typeof(CheckPoint))]
public class CheckPointDrawer : InteractableEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CheckPoint manager = (CheckPoint)target;
        SerializedObject so = new(manager);

        SerializedProperty _checkPointId = so.FindProperty("_checkPointId");
        EditorGUILayout.PropertyField(_checkPointId);

        SerializedProperty _cameraKey = so.FindProperty("_camKey");
        EditorGUILayout.PropertyField(_cameraKey);

        SerializedProperty _areaDistance = so.FindProperty("_areaMaxDistance");
        EditorGUILayout.PropertyField(_areaDistance);

        so.ApplyModifiedProperties();
    }
}