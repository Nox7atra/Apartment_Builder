using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
{
    public abstract class StateApartmentBuilder
    {
        #region attributes
        protected bool _IsActive;
        protected ApartmentEditorWindow _ParentWindow;
        #endregion
#region public methods
        public virtual void SetActive(bool enable)
        {
            _IsActive = enable;
        }

        public void Destroy()
        {
            _ParentWindow.onKeyEvent -= OnKeyEvent;
        }
        protected void DrawMouseLabel(Vector2 position)
        {
            Handles.color = Color.green;
            Handles.Label(position + MouseLabelOffset, _ParentWindow.Grid.GUIToGrid(position).ToString());
        }
        public abstract void Draw();
        #endregion

        #region events
        protected abstract void OnKeyEvent(EventType type, Event @event);
        #endregion
        #region construction
        protected StateApartmentBuilder(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _ParentWindow.onKeyEvent += OnKeyEvent;
            _IsActive = false;
        }
        #endregion


        #region constants
        private static readonly Vector2 MouseLabelOffset = new Vector2(10, 10);
        #endregion
    }
}