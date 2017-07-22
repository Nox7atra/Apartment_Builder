using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region attributes
        private Vector3? _lastMousePosition;
        #endregion

        #region service methods
        void DragGrid()
        {
            var curMousePosition = Event.current.mousePosition;
            if (_lastMousePosition.HasValue)
            {
                var dv = GUIUtility.GUIToScreenPoint((Vector2) _lastMousePosition)
                         - GUIUtility.GUIToScreenPoint(curMousePosition);
                _parentWindow.Grid.Move(dv);
                _parentWindow.Repaint();
            }
            _lastMousePosition = curMousePosition;
        }

        #endregion

        #region events

        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;
            switch (type)
            {
                case EventType.MouseDrag:
                    DragGrid();
                    break;
                case EventType.MouseDown:
                    _lastMousePosition = null;
                    break;
            }
        }

        #endregion


        #region constructors

        public NormalState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }

        #endregion
    }
}