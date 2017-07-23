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
                Handles.DrawLine(
                    mousePos,
                    _CurrentRoom.Contour[_CurrentRoom.Contour.Count - 1]
                    );
            }
            Handles.Label(mousePos, mousePos.ToString());

            _CurrentRoom.Draw(false);
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
                    if (_CurrentRoom.Contour.Count > 0 
                        && Vector2.Distance( _CurrentMousePosition , _CurrentRoom.Contour[0]) < Room.SNAPING_RAD)
                    {
                        if (_CurrentRoom.Contour.Count > 2)
                        {
                            _ParentWindow.CreateRoomEnd(_CurrentRoom);
                        }
                    }
                    else
                    {
                        _CurrentRoom.Contour.Add(_CurrentMousePosition);
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