using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentEditor
{
    public abstract class StateApartmentBuilder
    {
        protected bool _IsActive;
        protected ApartmentEditorWindow _ParentWindow;

        protected StateHotkeys _Hotkeys;
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
            WindowObjectDrawer.DrawLabel(_ParentWindow.Grid.GUIToGrid(position) + MouseLabelOffset, _ParentWindow.Grid.GUIToGrid(position).ToString());
        }
        public abstract void Draw();

        protected virtual void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode code)
        {
            switch (type)
            {
                case EventType.KeyDown:
                    if(code == KeyCode.Escape)
                        Reset();
                    break;
            }
        }

        protected StateApartmentBuilder(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _ParentWindow.OnKeyEvent += OnKeyEvent;
            _IsActive = false;
            
        }

        public abstract void Reset();

        private static readonly Vector2 MouseLabelOffset = new Vector2(10, 10);
    }
}