using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public sealed class ApartmentEditorWindow : EditorWindow
    {
        #region factory
        [MenuItem("Window/ApartmentEditor")]
        public static void Create()
        {
            var window = GetWindow<ApartmentEditorWindow>("ApartmentEditor");
            window.Show();
        }
        #endregion

        #region types
        public enum EditorWindowState
        {
            Normal,
            RoomCreation,
        }
        #endregion

        #region callbacks

        public event Action<EventType> onKeyEvent;
        #endregion

        #region attributes
        public readonly Grid Grid;


        private int _controlID;

        private Dictionary<EditorWindowState, StateApartmentEditor> _states;
        #endregion

        #region service methods
        void ActivateState(EditorWindowState state)
        {
            foreach (var stateApartmentEditor in _states)
            {
                stateApartmentEditor.Value.MouseCallbacks(stateApartmentEditor.Key == state);
            }
        }

    
        #endregion

        #region mouse
        void MouseEvents()
        {
            var curEvent = Event.current;
            switch (curEvent.type)
            {
                case EventType.MouseDown:
                    _controlID = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = _controlID;
                    break;
      
                case EventType.ScrollWheel:
                    OnScroll(curEvent.delta.y);
                    break;
           
            }
            
            if (curEvent.isMouse || curEvent.isKey || curEvent.isScrollWheel)
            {
                if(onKeyEvent != null)
                    onKeyEvent(curEvent.type);
            }
            else if (GUIUtility.hotControl == _controlID && Event.current.rawType == EventType.MouseUp)
            {
                if (onKeyEvent != null)
                    onKeyEvent(Event.current.rawType);
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
            MouseEvents();
            Grid.Draw();
        }

        void OnDestroy()
        {
            foreach (var stateApartmentEditor in _states.Values)
            {
                stateApartmentEditor.Destroy();
            }
        }
        #endregion


        #region constructors
        public ApartmentEditorWindow()
        {
            Grid = new Grid(this);
            _states = new Dictionary<EditorWindowState, StateApartmentEditor>();
            _states.Add(EditorWindowState.Normal, new NormalStateApartmentEditor(this));
            ActivateState(EditorWindowState.Normal);
        }
        #endregion

     
    }
}
