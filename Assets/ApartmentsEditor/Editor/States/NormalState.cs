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

        private int _SelectedVertIndex;
        private bool _IsPositionsShowed;
        private Vector3? _LastMousePosition;
        #endregion
        #region public methods
        public override void Draw()
        {
            if (!_IsActive)
                return;
            DrawSelection();
        }
        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            _LastMousePosition = null;
            _SelectedVertIndex = -1;
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
                    if (Event.current.button == 0)
                    {
                        var mousePos = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);

                        _SelectedRoom = _ParentWindow.CurrentApartment.Rooms
                            .FirstOrDefault(x => x.GetContourVertIndex(mousePos) >= 0);

                        if(_SelectedRoom != null)
                        {
                            _SelectedVertIndex = _SelectedRoom.GetContourVertIndex(mousePos);
                            ApartmentConfigWindow.MakeBackup();
                            ApartmentConfigWindow.Config.IsDrawPositions = true;
                        }
                        else
                        {
                            _SelectedRoom = _ParentWindow.CurrentApartment.Rooms
                                .FirstOrDefault(x => MathUtils.IsPointInsideCountour(x.Contour, mousePos));
                        }
                    }
                    _LastMousePosition = @event.mousePosition;
                    break;
                case EventType.MouseDrag:    
                    if (Event.current.button == 0)
                    {
                        DragSelected();
                    }
                    break;
                case EventType.MouseUp:
                    if (Event.current.button == 0)
                    {
                        ApartmentConfigWindow.ApplyBackup();
                        _LastMousePosition = null;
                        _SelectedVertIndex = -1;
                    }
                    break;
            }

            _ParentWindow.Repaint();
        }
        #endregion

        #region service methods
        void DragSelected()
        {
            if (_SelectedRoom == null)
                return;
            var curMousePosition = Event.current.mousePosition;

            if (_LastMousePosition.HasValue)
            {
                var dv = (GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                     - GUIUtility.GUIToScreenPoint(curMousePosition)) * _ParentWindow.Grid.Zoom;
                if (_SelectedVertIndex >= 0)
                {
                    _SelectedRoom.MoveVert(_SelectedVertIndex, -dv);
                }
                else
                {
                    _SelectedRoom.Move(-dv);
                }
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