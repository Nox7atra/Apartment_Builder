using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region properties
        public Room SelectedRoom{ get { return _SelectedRoom; }  }
        public SelectionType CurrentSelection { get { return _CurrentSelection; } }
        public int SelectedVertIndex { get { return _SelectedVertIndex; } }
        
        #endregion

        #region attributes
        private Room _SelectedRoom;
        private SelectionType _CurrentSelection;
        private int _SelectedVertIndex;

        private bool _IsPositionsShowed;
        private Vector3? _LastMousePosition;
        private NormalStateHotkeys _Hotkeys;
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
        public void DeleteSelected()
        {
            switch (_CurrentSelection)
            {
                case SelectionType.Room:
                    _ParentWindow.ApartmentManager.CurrentApartment.Rooms.Remove(_SelectedRoom);
                    _ParentWindow.ApartmentManager.SaveCurrent();
                    _SelectedRoom = null;
                    _ParentWindow.ApartmentManager.NeedToSave = true;
                    break;
                case SelectionType.Vert:
                    if (_SelectedRoom.Contour.Count > 3)
                    {
                        _SelectedRoom.Contour.RemoveAt(_SelectedVertIndex);
                        _SelectedVertIndex = -1;
                        _ParentWindow.ApartmentManager.NeedToSave = true;
                    }
                    else
                    {
                        Debug.LogWarning("You can't delete vertices when it less or equals 3");
                    }
                    break;
            }
            _CurrentSelection = SelectionType.None;
        }
        #endregion

        #region events
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;

            if (Event.current.button == 0)
            {
                OnLeftMouse(type, @event);
            }

            if(type == EventType.KeyDown)
            {
                 _Hotkeys.Use(@event);
            }
            _ParentWindow.Repaint();
        }
        #endregion

        #region service methods
        void MoveSelected()
        {
            if (_CurrentSelection == SelectionType.None)
                return;

            _ParentWindow.ApartmentManager.NeedToSave = true;

            var curMousePosition = Event.current.mousePosition;

            if (_LastMousePosition.HasValue)
            {
                var dv = (GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                     - GUIUtility.GUIToScreenPoint(curMousePosition)) * _ParentWindow.Grid.Zoom;
                switch (_CurrentSelection)
                {
                    case SelectionType.Vert:
                        _SelectedRoom.Contour[_SelectedVertIndex] -= dv;
                        break;
                    case SelectionType.Room:
                        _SelectedRoom.Move(-dv);
                        break;
                }
            }
            _LastMousePosition = curMousePosition;
        }
        void DrawSelection()
        {
            if (_SelectedRoom == null)
                return;
            Handles.color = Color.yellow;
            switch (_CurrentSelection)
            {
                case SelectionType.Vert:
                    Handles.DrawWireDisc(
                        _ParentWindow.Grid.GridToGUI(_SelectedRoom.Contour[_SelectedVertIndex]),
                        Vector3.back, Room.SNAPING_RAD / _ParentWindow.Grid.Zoom
                        );
                    break;
                case SelectionType.Room:
                    var contour = _SelectedRoom.Contour;
                    for (int i = 0; i < contour.Count; i++)
                    {
                        var p1 = _ParentWindow.Grid.GridToGUI(contour[i]);
                        var p2 = _ParentWindow.Grid.GridToGUI(contour[(i + 1) % contour.Count]);
                        Handles.DrawLine(p1, p2);
                    }
                    break;
            }
        }
        void SetupSelection(Vector2 position)
        {
            switch (_CurrentSelection)
            {
                case SelectionType.Room:
                    _SelectedRoom = _ParentWindow.ApartmentManager.CurrentApartment.Rooms
                                .FirstOrDefault(x => MathUtils.IsPointInsideCountour(x.Contour, position));
                    if(_SelectedRoom == null)
                    {
                        _CurrentSelection = SelectionType.None;
                    }
                    break;
                case SelectionType.Vert:
                    _SelectedVertIndex = _SelectedRoom.GetContourVertIndex(position);
                    ApartmentConfigWindow.MakeBackup();
                    ApartmentConfigWindow.Config.IsDrawPositions = true;
                    break;
            }  
        }
        void OnLeftMouse(EventType type, Event @event)
        {
            switch (type)
            {
                case EventType.MouseDown:
                    _CurrentSelection = SelectionType.None;

                    var mousePos = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);

                    _SelectedRoom = _ParentWindow.ApartmentManager.CurrentApartment.Rooms
                        .FirstOrDefault(x => x.GetContourVertIndex(mousePos) >= 0);

                    _CurrentSelection = _SelectedRoom == null ? SelectionType.Room : SelectionType.Vert;

                    SetupSelection(mousePos);
                    
                    _LastMousePosition = @event.mousePosition;
                    break;
                case EventType.MouseDrag:
                    MoveSelected();
                    break;
                case EventType.MouseUp:
                    ApartmentConfigWindow.ApplyBackup();
                    if(_SelectedRoom != null)
                        _SelectedRoom.RoundContourPoints();
                    _LastMousePosition = null;
                    break;
            }
        }
        #endregion

        #region nested types
        public enum SelectionType
        {
            Vert,
            Room,
            None
        }
        #endregion

        #region constructors
        public NormalState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
            _Hotkeys = new NormalStateHotkeys(this);
        }
        #endregion
    }
}