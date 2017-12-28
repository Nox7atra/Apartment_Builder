using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class AddObjectState : StateApartmentBuilder
    {
        #region fields
        
        private Vector2 _CurrentMousePosition;

        #endregion

        #region activation

        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            
            if (!enable)
            {
                return;
            }
            
        }

        #endregion


        #region drawing
        public override void Draw()
        {
            var manager = ObjectsManager.Instance;
            if(manager.SelectedObject != null)
                manager.SelectedObject.Draw(_ParentWindow.Grid.GUIToGrid(_CurrentMousePosition));
        }


        #endregion
       
        #region key events

        protected override void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode keyCode)
        {
            if (!_IsActive)
                return;
            base.OnKeyEvent(type, mousePosition, keyCode);
            switch (type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0)
                    {
                        ObjectsManager.Instance.SelectedObject.TryAddObject(_ParentWindow.Grid.GUIToGrid(_CurrentMousePosition));
                    }
                    break;
                case EventType.MouseMove:
                    _CurrentMousePosition = mousePosition;
                    _ParentWindow.Repaint();
                    break;
            }
        }

        #endregion

        #region construcor
        public AddObjectState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }
#endregion

        protected override void Reset()
        {
            ObjectsManager.Instance.Reset();
            ActiveEditorTracker.sharedTracker.isLocked = false;
            AssetDatabase.SaveAssets();
            _ParentWindow.ActivateState(ApartmentEditorWindow.EditorWindowState.Normal);
        }
    }
}