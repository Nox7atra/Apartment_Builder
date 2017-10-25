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
                _CurrentRoom = Room.Create();
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
            if (_CurrentRoom != null && _CurrentRoom.Walls.Count > 0)
            {
                Handles.color = Color.gray;
                var walls = _CurrentRoom.Walls;
                Handles.DrawLine(
                    mousePos,
                    _ParentWindow.Grid.GridToGUI(walls[walls.Count - 1].End)
                );
            }

            DrawMouseLabel(mousePos);

            _CurrentRoom.Draw(_ParentWindow.Grid);
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
                        if (_CurrentRoom.Add(_ParentWindow.Grid.GUIToGrid(@event.mousePosition)))
                        {
                            _ParentWindow.CreateRoomStateEnd(_CurrentRoom);
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