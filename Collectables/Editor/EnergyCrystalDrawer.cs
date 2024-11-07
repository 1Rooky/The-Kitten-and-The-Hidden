using UnityEditor;

[CustomEditor(typeof(EnergyCrystal))]
public class EnergyCrystalDrawer : InteractableEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnergyCrystal manager = (EnergyCrystal)target;
        SerializedObject so = new(manager);

        SerializedProperty _energyCrystalScriptProperty = so.FindProperty("_energyAmount");
        EditorGUILayout.PropertyField(_energyCrystalScriptProperty);

        so.ApplyModifiedProperties();
    }
}