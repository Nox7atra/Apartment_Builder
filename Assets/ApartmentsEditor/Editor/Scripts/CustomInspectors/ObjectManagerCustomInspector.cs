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

        private int _CurrentSelection;

        private WallObject _ChosenObject;
        public void OnEnable()
        {   
            _SelectedManager = target as ObjectsManager;

            _SelectedManager.OnReset += Repaint;
            if (_SelectedManager.CurrentMode == ObjectsManager.Mode.None)
                return;
            ActiveEditorTracker.sharedTracker.isLocked = true;
            CreateDefault();
        }
        public void OnDestroy()
        {
            _SelectedManager.OnReset -= Repaint;
            _SelectedManager = null;        
        }
        public override void OnInspectorGUI()
        {
            if (_SelectedManager.CurrentMode == ObjectsManager.Mode.None)
                return;

            base.OnInspectorGUI();
 
            _ChosenObject = (WallObject) EditorGUILayout.ObjectField(_ChosenObject, typeof(WallObject), false);
            _SelectedManager.SelectObject(_ChosenObject);
        }

        private void CreateDefault()
        {
            _ChosenObject = _SelectedManager.CurrentMode  == ObjectsManager.Mode.Doors ?
                (WallObject)Door.CreateOrGet("default", 190f, 80f) 
                : Window.CreateOrGet("default", 142f, 147f, 90f);
        }
    }
}