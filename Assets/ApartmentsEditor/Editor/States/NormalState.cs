using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region attributes
        private Room _SelectedRoom;

        private Vector3? _LastMousePosition;
        #endregion
        #region public methods
        public override void Draw()
        {
            if (!_IsActive)
                return;
            DrawSelection();
        }
        #endregion
        #region events
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;
            switch (type)
            {
                case EventType.MouseDown:
                    var mousePos = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);
                    if (Event.current.button == 0)
                    {
                        if (_ParentWindow.CurrentApartment.Rooms.Count > 0)
                        {
                            _SelectedRoom = _ParentWindow.CurrentApartment.Rooms
                                .FirstOrDefault(x => MathUtils.IsPointInsideCountour(x.Contour, mousePos));
                            _ParentWindow.Repaint();
                        }
                    }
                    _LastMousePosition = @event.mousePosition;
                    break;
                case EventType.MouseDrag:
                    if (Event.current.button == 0)
                    {
                        DragSelectedRoom();
                    }
                    break;
                case EventType.MouseUp:
                    if (Event.current.button == 0)
                    {
                        _LastMousePosition = null;
                    }
                    break;
            }
        }
        #endregion
#region service methods
        void DragSelectedRoom()
        {
            if (_SelectedRoom == null)
                return;
            var curMousePosition = Event.current.mousePosition;
            if (_LastMousePosition.HasValue)
            {
                var dv = GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                         - GUIUtility.GUIToScreenPoint(curMousePosition);
                _SelectedRoom.Move(-dv);
                _ParentWindow.Repaint();
            }
            _LastMousePosition = curMousePosition;
        }
        void DrawSelection()
        {
            if (_SelectedRoom == null)
                return;
            var contour = _SelectedRoom.Contour;
            for (int i = 0; i < contour.Count; i++)
            {
                var p1 = _ParentWindow.Grid.GridToGUI(contour[i]);
                var p2 = _ParentWindow.Grid.GridToGUI(contour[(i + 1) % contour.Count]);
                Handles.color = Color.yellow;
                Handles.DrawLine(p1, p2);
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