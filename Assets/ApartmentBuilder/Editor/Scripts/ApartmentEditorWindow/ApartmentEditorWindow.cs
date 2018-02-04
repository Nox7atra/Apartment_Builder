using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    public sealed class ApartmentBuilderWindow : EditorWindow
    {
        #region factory
        [MenuItem("Window/Apartment Builder/Open Editor")]
        public static void Create()
        {
            var window = GetWindow<ApartmentBuilderWindow>("ApartmentBuilder");
            window.position = new Rect(100, 100, 400, 400);
            window.Show();
        }
        #endregion

        #region events
        public event Action<EventType, Vector2, KeyCode> OnKeyEvent;
        #endregion
        
        #region fields

        public ApartmentBuilderGrid Grid;

        private Toolbar _Toolbar;
        private EditorWindowState _CurrentState;
        private Dictionary<EditorWindowState, StateApartmentBuilder> _States;
        private Vector3? _LastMousePosition;
        #endregion

        #region properties

        public EditorWindowState CurrentState
        {
            get { return _CurrentState; }
        }

        #endregion

        #region object to add state

        public void AddObjectStateBegin(ObjectsManager.Mode mode)
        {
            if (ApartmentsManager.Instance.CurrentApartment.Rooms.Count <= 0)
            {
                Debug.Log("Create at least one room to add " + mode.ToString());
                return;
            }
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
            _CurrentState = state;
            foreach (var stateApartmentBuilder in _States)
            {
                stateApartmentBuilder.Value.SetActive(stateApartmentBuilder.Key == state);
            }
        }

        #endregion

        #region engine methods

        private void OnDestroy()
        {
            foreach (var stateApartmentBuilder in _States.Values)
            {
                stateApartmentBuilder.Destroy();
            }
        }
        private void OnEnable()
        {
            Grid = new ApartmentBuilderGrid(this);
            _Toolbar = new Toolbar(this);
            WindowObjectDrawer.CurrentWindow = this;
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
            float bgValue = 56 / 255f;
            GUI.color = new Color(bgValue, bgValue, bgValue);
            GUI.DrawTexture(new Rect(Vector2.zero, maxSize), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;

            Grid.Draw();


            var apartment = ApartmentsManager.Instance.CurrentApartment;
            if (apartment != null)
            {
                apartment.Draw();
            }

            foreach (var stateApartmentBuilder in _States)
            {
                stateApartmentBuilder.Value.Draw();
            }

            _Toolbar.Draw();
            KeysEvents();
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
