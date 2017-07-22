using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public sealed class ApartmentEditorWindow : EditorWindow
    {
        #region factory
        [MenuItem("Window/ApartmentBuilder")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentBuilder");
            window.Show();
        }
        #endregion

        #region nested types
        public enum EditorWindowState
        {
            Normal,
            RoomCreation
        }
        #endregion

        #region callbacks
        public event Action<EventType, Event> onKeyEvent;
        #endregion
        #region properties
        public Apartment CurrentApartment
        {
            get
            {
                return _CurrentApartment;
            }
        }
        #endregion
        #region attributes
        public readonly Grid Grid;

        private readonly Toolbar _Toolbar;

        private Apartment _CurrentApartment;
        private int _ControlId;
        private readonly Dictionary<EditorWindowState, StateApartmentBuilder> _States;
        #endregion

        #region service methods
        void ActivateState(EditorWindowState state)
        {
            foreach (var stateApartmentEditor in _States)
            {
                stateApartmentEditor.Value.MouseCallbacks(stateApartmentEditor.Key == state);
            }
        }
        #endregion

        #region keys
        void KeysEvents()
        {
            var curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.MouseDown:
                    _ControlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = _ControlId;
                    break;
                case EventType.ScrollWheel:
                    OnScroll(curEvent.delta.y);
                    break;
            }

            if (onKeyEvent != null)
            {
                onKeyEvent(curEvent.type, curEvent);
            }
            else if (GUIUtility.hotControl == _ControlId && Event.current.rawType == EventType.MouseUp)
            {
                if (onKeyEvent != null)
                    onKeyEvent(Event.current.rawType, curEvent);
            }
        }

        void OnScroll(float speed)
        {
            Grid.Zoom(speed);
            Repaint(); 
        }

        #endregion

        #region engine methods
        void OnGUI()
        {
            KeysEvents();
            Grid.Draw();
            _Toolbar.Draw();
        }

        void OnDestroy()
        {
            foreach (var stateApartmentEditor in _States.Values)
            {
                stateApartmentEditor.Destroy();
            }
        }
        #endregion


        #region constructors
        public ApartmentEditorWindow()
        {
            Grid     = new Grid(this);
            _Toolbar = new Toolbar(this);

            _States = new Dictionary<EditorWindowState, StateApartmentBuilder>
            {
                {EditorWindowState.Normal,       new NormalState(this)},
                {EditorWindowState.RoomCreation, new CreatingRoomState(this)}
            };
            ActivateState(EditorWindowState.Normal);
        }
        #endregion

     
    }
}
