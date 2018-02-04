using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    [CustomEditor(typeof(WallObject), true)]
    public class WallObjectCustomInspector : Editor
    {
        private WallObject _ThisWallObject;

        private void OnEnable()
        {
            _ThisWallObject = (WallObject) target;
        }

        public override void OnInspectorGUI()
        {
            TopButtons();
            base.OnInspectorGUI();
        }
        private void TopButtons()
        {
            GUILayout.BeginHorizontal();
            CreateNew();
            GUILayout.EndHorizontal();
        }

        private void CreateNew()
        {
            if (GUILayout.Button(
                "Create new"
            ))
            {
                var type = _ThisWallObject.GetType();
                typeof(ScriptableObjectUtils).GetMethod("CreateOrGet",
                        BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(type)
                    .Invoke(null, 
                    new object[]
                    {
                        (object )("New " + type.Name + GUID.Generate())
                    }
                    );
            }
        }
    }
}