using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public sealed class ApartmentEditorWindow : EditorWindow
    {
        [MenuItem("Window/ApartmentBuilder")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentBuilder");
            window.Show();
        }

        public event Action<EventType, Event> OnKeyEvent;

        public ApartmentsManager ApartmentManager
        {
            get
            {
                return _ApartmentManager;
            }
        }

        public  Grid Grid;

        private Toolbar _Toolbar;
        private ApartmentsManager _ApartmentManager;
        private Dictionary<EditorWindowState, StateApartmentBuilder> _States;
        private Vector3? _LastMousePosition;


        void OnGUI()
        {
            KeysEvents();
            Grid.Draw();
            var apartment = _ApartmentManager.CurrentApartment;
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
        void OnEnable()
        {
            Grid = new Grid(this);
            _Toolbar = new Toolbar(this);
            _ApartmentManager = new ApartmentsManager();

            _States = new Dictionary<EditorWindowState, StateApartmentBuilder>
            {
                {EditorWindowState.Normal,       new NormalState(this)},
                {EditorWindowState.RoomCreation, new CreatingRoomState(this)}
            };
            ActivateState(EditorWindowState.Normal);

            wantsMouseMove = true;
        }
        void OnDestroy()
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
            _ApartmentManager.CurrentApartment.Rooms.Add(room);
            ActivateState(EditorWindowState.Normal);
            _ApartmentManager.SaveCurrent();
            Repaint();
        }

        void KeysEvents()
        {
            var curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.MouseDrag:
                    if (Event.current.button == 1)
                        DragGrid();
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 1)
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
        void DragGrid()
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
        void OnScroll(float speed)
        {
            Grid.Zoom += speed * Grid.Zoom * 0.1f;
            Repaint();
        }

        void ActivateState(EditorWindowState state)
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
