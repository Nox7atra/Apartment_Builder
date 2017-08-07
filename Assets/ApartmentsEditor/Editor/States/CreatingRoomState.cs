using System;
using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
{
    public class CreatingRoomState : StateApartmentBuilder
    {
        #region attributes
        private Room _CurrentRoom;
        private Vector2 _CurrentMousePosition;
        #endregion

        #region public methods
        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            if (enable)
            {
                _CurrentRoom = new Room();
            }
            else
            {
                _CurrentRoom = null;
            }
        }

        public override void Draw()
        {
            if (!_IsActive)
                return;
            var mousePos = _CurrentMousePosition;
            if (_CurrentRoom != null && _CurrentRoom.Contour.Count > 0)
            {
                Handles.color = Color.gray;
                Handles.DrawLine(
                    mousePos,
                    _ParentWindow.Grid.GridToGUI(_CurrentRoom.Contour[_CurrentRoom.Contour.Count - 1])
                    );
            }

            DrawMouseLabel(mousePos);

            _CurrentRoom.Draw(_ParentWindow.Grid, false);
        }
        #endregion

        #region key events
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;
            switch (type)
            {
                case EventType.MouseMove:
                    _CurrentMousePosition = @event.mousePosition;
                    _ParentWindow.Repaint();
                    break;
                case EventType.MouseDown:
                    if (@event.button == 0)
                    {
                        var posToAdd = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);
                        if (_CurrentRoom.Contour.Count > 0
                            && _CurrentRoom.IsLastPoint(posToAdd))
                        {
                            if (_CurrentRoom.Contour.Count > 2)
                            {
                                _ParentWindow.CreateRoomEnd(_CurrentRoom);
                            }
                        }
                        else
                        {
                            _CurrentRoom.Contour.Add(posToAdd);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region constructor
        public CreatingRoomState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }
        #endregion
    }
}