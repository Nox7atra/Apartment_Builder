using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public sealed class ApartmentEditorWindow : EditorWindow
    {
        [MenuItem("Window/ApartmentBuilder/OpenEditor")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentBuilder");
            window.Show();
            ApartmentsManagerWindow.Create();
        }

        public event Action<EventType, Event> OnKeyEvent;

        public  Grid Grid;

        private Toolbar _Toolbar;
        private Dictionary<EditorWindowState, StateApartmentBuilder> _States;
        private Vector3? _LastMousePosition;


        private void OnGUI()
        {
            KeysEvents();
            Grid.Draw();
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            if (apartment != null)
            {
                apartment.Draw(Grid);
            }

            foreach (var stateApartmentEditor in _States)
            {
                stateApartmentEditor.Value.Draw();
            }

            _Toolbar.Draw();
        }

        private void OnEnable()
        {
            Grid = new Grid(this);
            _Toolbar = new Toolbar(this);

            _States = new Dictionary<EditorWindowState, StateApartmentBuilder>
            {
                {EditorWindowState.Normal,       new NormalState(this)},
                {EditorWindowState.RoomCreation, new CreatingRoomState(this)}
            };
            ActivateState(EditorWindowState.Normal);

            wantsMouseMove = true;
        }

        private void OnDestroy()
        {
            foreach (var stateApartmentEditor in _States.Values)
            {
                stateApartmentEditor.Destroy();
            }
        }

        public void CreateRoomStateBegin()
        {
            ActivateState(EditorWindowState.RoomCreation);
        }
        public void CreateRoomStateEnd(Room room)
        {
            room.MakeClockwiseOrientation();
            ApartmentsManager.Instance.CurrentApartment.Rooms.Add(room);
            ActivateState(EditorWindowState.Normal);
            Repaint();
        }

        private void KeysEvents()
        {
            var curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.MouseDrag:
                    if (curEvent.button == 1)
                        DragGrid();
                    break;
                case EventType.MouseDown:
                    if (curEvent.button == 1)
                        _LastMousePosition = null;
                    break;
                case EventType.ScrollWheel:
                    OnScroll(curEvent.delta.y);
                    break;
            }

            if (OnKeyEvent != null)
            {
                OnKeyEvent(curEvent.type, curEvent);
            }
            else if (Event.current.rawType == EventType.MouseUp)
            {
                if (OnKeyEvent != null)
                    OnKeyEvent(Event.current.rawType, curEvent);
            }
        }

        private void DragGrid()
        {
            var curMousePosition = Event.current.mousePosition;
            if (_LastMousePosition.HasValue)
            {
                var dv = GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                         - GUIUtility.GUIToScreenPoint(curMousePosition);
                Grid.Move(dv);
                Repaint();
            }
            _LastMousePosition = curMousePosition;
        }

        private void OnScroll(float speed)
        {
            Grid.Zoom += speed * Grid.Zoom * 0.1f;
            Repaint();
        }

        public void ActivateState(EditorWindowState state)
        {
            foreach (var stateApartmentEditor in _States)
            {
                stateApartmentEditor.Value.SetActive(stateApartmentEditor.Key == state);
            }
        }
        
     
        public enum EditorWindowState
        {
            Normal,
            RoomCreation
        }
    }
}
