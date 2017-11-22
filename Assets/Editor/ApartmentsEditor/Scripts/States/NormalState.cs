using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        public Room SelectedRoom{ get { return _SelectedRoom; }  }
        public SelectionType CurrentSelection { get { return _CurrentSelection; } }
        public int SelectedVertIndex { get { return _SelectedVertIndex; } }
   
        private Room _SelectedRoom;
        private SelectionType _CurrentSelection;
        private int _SelectedVertIndex;

        private Vector3? _LastMousePosition;
        private NormalStateHotkeys _Hotkeys;
        private Apartment _CurrentApartment;
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
                    ApartmentsManager.Instance.CurrentApartment.Rooms.Remove(_SelectedRoom);
                    UnityEngine.Object.DestroyImmediate(_SelectedRoom, true);
                    _SelectedRoom = null;

                   
                    break;
                case SelectionType.Vert:
                    if (_SelectedRoom.Walls.Count > 3)
                    {
                        Undo.RegisterCompleteObjectUndo(_SelectedRoom, "Vert Removed");
                        _SelectedRoom.RemoveVert(_SelectedVertIndex);
                        _SelectedVertIndex = -1;
                        ApartmentsManager.Instance.NeedToSave = true;
                    }
                    else
                    {
                        Debug.LogWarning("You can't delete vertices when it less or equals 3");
                    }
                    break;
            }
            AssetDatabase.SaveAssets();
            _CurrentSelection = SelectionType.None;
        }
 
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

        void MoveSelected()
        {
            if (_CurrentSelection == SelectionType.None)
                return;
            
            ApartmentsManager.Instance.NeedToSave = true;

            var curMousePosition = Event.current.mousePosition;
    
            if (_LastMousePosition.HasValue)
            {
                var dv = (GUIUtility.GUIToScreenPoint((Vector2)_LastMousePosition)
                     - GUIUtility.GUIToScreenPoint(curMousePosition)) * _ParentWindow.Grid.Zoom;
                switch (_CurrentSelection)
                {
                    case SelectionType.Vert:
                        _SelectedRoom.MoveVert(_SelectedVertIndex, -dv);
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
  
            switch (_CurrentSelection)
            {
                case SelectionType.Vert:
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(
                        _ParentWindow.Grid.GridToGUI(_SelectedRoom.GetVertPosition(_SelectedVertIndex)),
                        Vector3.back, Room.SNAPING_RAD / _ParentWindow.Grid.Zoom
                    );
                    break;
                case SelectionType.Room:
                    var walls = _SelectedRoom.Walls;
                    for (int i = 0; i < walls.Count; i++)
                    {
                        walls[i].Draw(_ParentWindow.Grid, Color.yellow);
                    }
                    break;
            }
        }
        void SetupSelection(Vector2 position)
        {
            switch (_CurrentSelection)
            {
                case SelectionType.Room:
                    _SelectedRoom = ApartmentsManager.Instance.CurrentApartment.Rooms
                               .FirstOrDefault(x => MathUtils.IsPointInsideCountour(x.GetContour(), position));
                    if(_SelectedRoom == null)
                    {
                        _CurrentSelection = SelectionType.None;
                    }
                    break;
                case SelectionType.Vert:
                    _SelectedVertIndex = _SelectedRoom.GetContourVertIndex(position);
                    ApartmentConfig.MakeBackup();
                    ApartmentConfig.Current.IsDrawPositions = true;
                    break;
            }
            if (_SelectedRoom)
            {
                Selection.activeObject = _SelectedRoom;
            }
        }
        void OnLeftMouse(EventType type, Event @event)
        {
            switch (type)
            {
                case EventType.MouseDown:
                    _CurrentSelection = SelectionType.None;

                    var mousePos = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);

                    _SelectedRoom = ApartmentsManager.Instance.CurrentApartment.Rooms
                        .FirstOrDefault(x => x.GetContourVertIndex(mousePos) >= 0);
                   
                    _CurrentSelection = _SelectedRoom == null ? SelectionType.Room : SelectionType.Vert;

                    SetupSelection(mousePos);
                    if (_SelectedRoom)
                        Undo.RegisterCompleteObjectUndo(_SelectedRoom, "Room position changed");
                    _LastMousePosition = @event.mousePosition;
                    break;
                case EventType.MouseDrag:
                    MoveSelected();
                    break;
                case EventType.MouseUp:
                    ApartmentConfig.ApplyBackup();
       
                    _LastMousePosition = null;
                    break;
            }
        }
 
        public enum SelectionType
        {
            Vert,
            Room,
            None
        }
    
        public NormalState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
            _Hotkeys = new NormalStateHotkeys(this);
        }
    }
}