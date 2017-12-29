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

        private IWallObject _ChosenObject;
        public void OnEnable()
        {   
            _SelectedManager = target as ObjectsManager;

            _SelectedManager.OnReset += Repaint;
            if (_SelectedManager.CurrentMode == ObjectsManager.Mode.None)
                return;
            ActiveEditorTracker.sharedTracker.isLocked = true;
            

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
                _ChosenObject = (WallObject) EditorGUILayout.ObjectField(wallObj, typeof(WallObject), false);
                _SelectedManager.SelectObject(_ChosenObject);
            }
        }
    }
}