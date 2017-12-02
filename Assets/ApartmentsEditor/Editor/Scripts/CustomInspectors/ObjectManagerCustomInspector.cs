using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Foxsys.ApartmentEditor
{
    [CustomEditor(typeof(ObjectsManager))]
    public class ObjectManagerCustomInspector : Editor
    {
        private ObjectsManager _SelectedManager;

        public void OnEnable()
        {
            _SelectedManager = target as ObjectsManager;
        }
        public void OnDestroy()
        {
            _SelectedManager = null;
        }
        public override void OnInspectorGUI()
        {
            if (_SelectedManager.CurrentMode == ObjectsManager.Mode.None)
                return;

            base.OnInspectorGUI();

        }
    }
}