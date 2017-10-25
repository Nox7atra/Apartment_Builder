using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
{
    public abstract class StateApartmentBuilder
    {
        protected bool _IsActive;
        protected ApartmentEditorWindow _ParentWindow;
     
        public void SaveCurrentApartment()
        {
        }
        public virtual void SetActive(bool enable)
        {
            _IsActive = enable;
        }

        public void Destroy()
        {
            _ParentWindow.OnKeyEvent -= OnKeyEvent;
        }
        protected void DrawMouseLabel(Vector2 position)
        {
            Handles.Label(position + MouseLabelOffset, _ParentWindow.Grid.GUIToGrid(position).ToString());
        }
        public abstract void Draw();

        protected abstract void OnKeyEvent(EventType type, Event @event);

        protected StateApartmentBuilder(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _ParentWindow.OnKeyEvent += OnKeyEvent;
            _IsActive = false;
        }

        private static readonly Vector2 MouseLabelOffset = new Vector2(10, 10);
    }
}