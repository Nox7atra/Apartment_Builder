using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    [CustomEditor(typeof(Skin))]
    public class SkinCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Default"))
            {
                ((Skin)target).RefreshDefaultStyles();
            }
            EditorGUILayout.EndHorizontal();
            base.OnInspectorGUI();
        }
    }
}