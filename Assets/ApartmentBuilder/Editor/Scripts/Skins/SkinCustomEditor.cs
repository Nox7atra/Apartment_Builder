using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
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