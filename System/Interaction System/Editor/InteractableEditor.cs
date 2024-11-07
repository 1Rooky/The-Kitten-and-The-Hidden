using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    private SerializedProperty autoInteractProperty;
    private SerializedProperty interactProperty;
    private SerializedProperty useEventsProperty;
    private SerializedProperty addSFXProperty;
    private SerializedProperty sfxProperty;

    public override void OnInspectorGUI()
    {
        Interactable _interactable = (Interactable)target;
        SerializedObject so = new(target);

        LayerMask newLayerMask = LayerMaskField(new GUIContent("Layers Interacted With"), _interactable.LayersInteractedWith);
        if (_interactable.LayersInteractedWith != newLayerMask)
        {
            _interactable.LayersInteractedWith = newLayerMask;
            EditorUtility.SetDirty(_interactable);
        }

        autoInteractProperty = so.FindProperty("_autoInteract");
        interactProperty = so.FindProperty("_interact");
        useEventsProperty = so.FindProperty("_useEvents");
        addSFXProperty = so.FindProperty("_AddSFX"); // Initialize this property
        sfxProperty = so.FindProperty("_sfx"); // Initialize this property

        if (!autoInteractProperty.boolValue)
            EditorGUILayout.PropertyField(interactProperty);

        if (!interactProperty.boolValue)
            EditorGUILayout.PropertyField(autoInteractProperty);

        // Draw the _useParticleEffect field
        EditorGUILayout.PropertyField(so.FindProperty("_useParticleEffect"));

        // If _useParticleEffect is true, draw the _interactionParticals field
        if (_interactable.UseParticleEffect)
            EditorGUILayout.PropertyField(so.FindProperty("_interactionParticals"));

        // Add SFX toggle and conditional EventReference field
        EditorGUILayout.PropertyField(addSFXProperty);
        if (addSFXProperty.boolValue) // Only show the EventReference field if _AddSFX is true
            EditorGUILayout.PropertyField(sfxProperty);

        if (autoInteractProperty.boolValue || interactProperty.boolValue)
        {
            if (interactProperty.boolValue)
            {
                SerializedProperty outlineMaterialProperty = so.FindProperty("_outlineMaterial");
                EditorGUILayout.PropertyField(outlineMaterialProperty);

                if (outlineMaterialProperty.objectReferenceValue != null)
                {
                    SerializedProperty renderersProperty = so.FindProperty("renderers");
                    EditorGUILayout.PropertyField(renderersProperty, true);
                }
            }

            EditorGUILayout.PropertyField(useEventsProperty);
        }
        else if (!autoInteractProperty.boolValue && !interactProperty.boolValue)
            useEventsProperty.boolValue = false;

        if (_interactable.gameObject.TryGetComponent<Collider>(out var collider))
            collider.isTrigger = true;
        else
        {
            collider = _interactable.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        if (interactProperty.boolValue)
        {
            EditorGUILayout.LabelField("Prompt Message");
            _interactable.PromptMessage = EditorGUILayout.TextArea(_interactable.PromptMessage, GUILayout.MaxHeight(75));
        }

        if (_interactable.UseEvents)
        {
            if (_interactable.gameObject.GetComponent<InteractableEvents>() == null)
                _interactable.gameObject.AddComponent<InteractableEvents>();
        }
        else
        {
            if (_interactable.gameObject.GetComponent<InteractableEvents>() != null)
                DestroyImmediate(_interactable.gameObject.GetComponent<InteractableEvents>());
        }

        so.ApplyModifiedProperties();
    }

    private List<string> GetAllLayers()
    {
        List<string> layers = new();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layers.Add(layerName);
            }
        }
        return layers;
    }

    private LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
    {
        var layers = GetAllLayers();

        List<int> layerNumbers = new();
        for (int i = 0; i < layers.Count; i++)
        {
            int layerNumber = LayerMask.NameToLayer(layers[i]);
            if (layerMask == (layerMask | (1 << layerNumber)))
            {
                layerNumbers.Add(i);
            }
        }

        int mask = 0;
        foreach (var number in layerNumbers)
        {
            mask |= (1 << number);
        }

        mask = EditorGUILayout.MaskField(label, mask, layers.ToArray());

        LayerMask correctedMask = 0;
        for (int i = 0; i < layers.Count; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                int layerIndex = LayerMask.NameToLayer(layers[i]);
                correctedMask |= (1 << layerIndex);
            }
        }

        return correctedMask;
    }
}