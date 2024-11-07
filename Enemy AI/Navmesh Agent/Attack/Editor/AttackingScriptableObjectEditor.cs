using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Custom editor for AttackingScriptableObject to provide a specialized inspector interface
[CustomEditor(typeof(AttackingScriptableObject))]
public class AttackingScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Cast the target of this editor to AttackingScriptableObject for easier access
        AttackingScriptableObject script = (AttackingScriptableObject)target;

        // Display a label for the attack configuration section
        EditorGUILayout.LabelField("Attack Configuration", EditorStyles.boldLabel);
        // Fields for editing basic attack properties
        script.Damage = EditorGUILayout.IntField("Damage", script.Damage);
        script.AttackRadius = EditorGUILayout.FloatField("Attack Radius", script.AttackRadius);
        script.AttackDelay = EditorGUILayout.FloatField("Attack Delay", script.AttackDelay);
        // Custom field for selecting a LayerMask
        script.LineOfSightLayers = LayerMaskField(new GUIContent("Layer Mask"), script.LineOfSightLayers);

        // Display a label for the ranged configuration section
        EditorGUILayout.LabelField("Ranged Configuration", EditorStyles.boldLabel);
        // Toggle for enabling/disabling ranged attack
        script.IsRanged = EditorGUILayout.Toggle("Is Ranged", script.IsRanged);

        if (script.IsRanged)
        {
            // Toggle for enabling/disabling homing bullets
            script.IsHomingBullet = EditorGUILayout.Toggle("Is Homing Bullet", script.IsHomingBullet);
            if (script.IsHomingBullet)
            {
                // Field for selecting the homing bullet prefab
                GameObject homingBulletPrefab = (GameObject)EditorGUILayout.ObjectField("Homing Bullet Prefab", script.BulletPrefab, typeof(GameObject), false);
                // Warning if the selected prefab does not have a HomingBullet component
                if (homingBulletPrefab != null && homingBulletPrefab.GetComponent<HomingBullet>() == null)
                {
                    EditorGUILayout.HelpBox("The selected GameObject does not have a HomingBullet component.", MessageType.Warning);
                }
                script.BulletPrefab = homingBulletPrefab;
            }
            else
            {
                // Field for selecting the bullet prefab
                GameObject bulletPrefab = (GameObject)EditorGUILayout.ObjectField("Bullet Prefab", script.BulletPrefab, typeof(GameObject), false);
                // Warning if the selected prefab does not have a Bullet component
                if (bulletPrefab != null && bulletPrefab.GetComponent<Bullet>() == null)
                {
                    EditorGUILayout.HelpBox("The selected GameObject does not have a Bullet component.", MessageType.Warning);
                }
                script.BulletPrefab = bulletPrefab;
            }

            // Field for editing the bullet spawn offset
            script.BulletSpawnOffset = EditorGUILayout.Vector3Field("Bullet Spawn Offset", script.BulletSpawnOffset);
        }

        // Mark the script as dirty and apply modified properties if any GUI element was changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            serializedObject.ApplyModifiedProperties();
        }
    }

    // Helper method to get all layer names
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

    // Custom field for selecting a LayerMask
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