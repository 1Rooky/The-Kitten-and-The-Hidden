using UnityEditor;

[CustomEditor(typeof(HealthCrystal))]
public class HealthCrystalDrawer : InteractableEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HealthCrystal manager = (HealthCrystal)target;
        SerializedObject so = new(manager);

        SerializedProperty _healthCrystalScriptProperty = so.FindProperty("_healAmount");
        EditorGUILayout.PropertyField(_healthCrystalScriptProperty);

        so.ApplyModifiedProperties();
    }
}