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

        private IWallObject _ChosenObject;
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
            if (_SelectedManager.CurrentMode == ObjectsManager.Mode.None || _SelectedManager.CurrentMode == ObjectsManager.Mode.Vert)
                return;

            base.OnInspectorGUI();
            var wallObj = _ChosenObject as WallObject;
            if (wallObj != null)
            {
                wallObj = (WallObject) EditorGUILayout.ObjectField(wallObj, typeof(WallObject), false);
                _SelectedManager.SelectObject(_ChosenObject);
            }
        }

        private void CreateDefault()
        {
            _ChosenObject = _SelectedManager.CurrentMode  == ObjectsManager.Mode.Doors ?
                (WallObject)Door.CreateOrGet("default", 80f, 190f) 
                : Window.CreateOrGet("default", 142f, 147f, 90f);
        }
    }
}