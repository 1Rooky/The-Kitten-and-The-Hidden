using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectType))]
public class ObjectTypeDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectType _objectType = (ObjectType)target;

        bool wasCat = _objectType.IsCat;
        bool wasGhost = _objectType.IsGhost;
        bool wasObject = _objectType.IsObject;

        if (!wasGhost && !wasObject)
            _objectType.IsCat = EditorGUILayout.Toggle("Is Cat", _objectType.IsCat);

        if (!wasCat && !wasObject)
            _objectType.IsGhost = EditorGUILayout.Toggle("Is Ghost", _objectType.IsGhost);

        if (!wasCat && !wasGhost)
            _objectType.IsObject = EditorGUILayout.Toggle("Is Object", _objectType.IsObject);

        if (_objectType.IsCat)
        {
            _objectType.IsGhost = false;
            _objectType.IsObject = false;
        }

        if (_objectType.IsGhost)
        {
            _objectType.IsCat = false;
            _objectType.IsObject = false;
        }

        if (_objectType.IsObject)
        {
            _objectType.IsCat = false;
            _objectType.IsGhost = false;
        }

        if (wasCat != _objectType.IsCat)
        {
            if (_objectType.IsCat)
            {
                _objectType.gameObject.AddComponent<Cat>();

                if (_objectType.gameObject.TryGetComponent<Ghost>(out var ghostComponent))
                    DestroyImmediate(ghostComponent);
            }
            else
            {
                if (_objectType.gameObject.TryGetComponent<Cat>(out var catComponent))
                    DestroyImmediate(catComponent);
            }
        }

        if (wasGhost != _objectType.IsGhost)
        {
            if (_objectType.IsGhost)
            {
                _objectType.gameObject.AddComponent<Ghost>();

                if (_objectType.gameObject.TryGetComponent<Cat>(out var catComponent))
                    DestroyImmediate(catComponent);
            }
            else
            {
                if (_objectType.gameObject.TryGetComponent<Ghost>(out var ghostComponent))
                    DestroyImmediate(ghostComponent);
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(_objectType);
    }
}