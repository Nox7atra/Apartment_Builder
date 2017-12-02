using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public sealed class ApartmentEditorWindow : EditorWindow
    {
        #region factory
        [MenuItem("Window/ApartmentBuilder/OpenEditor")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentBuilder");
            window.Show();
        }
        #endregion

        #region events
        public event Action<EventType, Vector2, KeyCode> OnKeyEvent;
        #endregion
        
        #region fields

        public Grid Grid;

        private Toolbar _Toolbar;
        private Dictionary<EditorWindowState, StateApartmentBuilder> _States;
        private Vector3? _LastMousePosition;

        #endregion

        #region object to add state

        public void AddObjectStateBegin(ObjectsManager.Mode mode)
        {
            ObjectsManager.Instance.SelectMode(mode);
            Selection.activeObject = ObjectsManager.Instance;
            ActivateState(EditorWindowState.ObjectAdding);
        }

        #endregion

        #region creating room state

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

        #endregion

        #region key events

        private void KeysEvents()
        {
            var curEvent = Event.current;
            var mousePosition = curEvent.mousePosition;
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
                OnKeyEvent(curEvent.type, mousePosition, curEvent.keyCode);
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
        #endregion

        #region activation

        public void ActivateState(EditorWindowState state)
        {
            foreach (var stateApartmentEditor in _States)
            {
                stateApartmentEditor.Value.SetActive(stateApartmentEditor.Key == state);
            }
        }

        #endregion

        #region engine methods

        private void OnDestroy()
        {
            foreach (var stateApartmentEditor in _States.Values)
            {
                stateApartmentEditor.Destroy();
            }
        }
        private void OnEnable()
        {
            Grid = new Grid(this);
            _Toolbar = new Toolbar(this);

            _States = new Dictionary<EditorWindowState, StateApartmentBuilder>
            {
                {EditorWindowState.Normal,       new NormalState(this)},
                {EditorWindowState.RoomCreation, new CreatingRoomState(this)},
                {EditorWindowState.ObjectAdding, new AddObjectState(this) }
            };
            ActivateState(EditorWindowState.Normal);

            wantsMouseMove = true;
        }

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
        #endregion

        public enum EditorWindowState
        {
            Normal,
            RoomCreation,
            ObjectAdding
        }
    }
}
