using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Foxsys.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region fields
        private SelectionType _CurrentSelection;
        private int _SelectedVertIndex;

        private Vector2? _LastMousePosition;
    #endregion
        public override void Draw()
        {
            if (!_IsActive)
                return;
            ApartmentsManager.Instance.CurrentApartment.SelectedRoom = Selection.activeObject as Room;
            DrawSelection();
        }
        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            _CurrentSelection = SelectionType.None;
            _LastMousePosition = null;
            _SelectedVertIndex = -1;
        }
        public void DeleteSelected()
        {

            switch (_CurrentSelection)
            {
                case SelectionType.Room:
                    ApartmentsManager.Instance.CurrentApartment.Rooms.Remove(ApartmentsManager.Instance.CurrentApartment.SelectedRoom);
                    UnityEngine.Object.DestroyImmediate(ApartmentsManager.Instance.CurrentApartment.SelectedRoom, true);
                    ApartmentsManager.Instance.CurrentApartment.SelectedRoom = null;
                    break;
                case SelectionType.Vert:
                    if (ApartmentsManager.Instance.CurrentApartment.SelectedRoom.Walls.Count > 3)
                    {
                        Undo.RegisterCompleteObjectUndo(ApartmentsManager.Instance.CurrentApartment.SelectedRoom, "Vert Removed");
                        ApartmentsManager.Instance.CurrentApartment.SelectedRoom.RemoveVert(_SelectedVertIndex);
                        _SelectedVertIndex = -1;
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
 
        protected override void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode keyCode)
        {
            if (!_IsActive)
                return;

            if (Event.current.button == 0)
            {
                OnLeftMouse(type, mousePosition);
            }

            if(type == EventType.KeyDown)
            {
                 _Hotkeys.Use(keyCode, true);
            }
            if (type == EventType.KeyUp)
            {
                _Hotkeys.Use(keyCode, false);
            }
            _ParentWindow.Repaint();
        }

        private void MoveSelected(Vector2 mousePositon)
        {

            if (_CurrentSelection == SelectionType.None )
                return;
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            if (_LastMousePosition.HasValue)
            {
                var dv = (GUIUtility.GUIToScreenPoint(_LastMousePosition.Value)
                     - GUIUtility.GUIToScreenPoint(mousePositon)) * _ParentWindow.Grid.Zoom;
                var dimensions = apartment.Dimensions;
                switch (_CurrentSelection)
                {
                    case SelectionType.Vert:
                        var gridMousePos = _ParentWindow.Grid.GUIToGrid(mousePositon);
                        var vertPos =
                            _Hotkeys.IsProjectionHotkeyPressed
                                ? apartment.GetPointProjectionOnNearestContour(gridMousePos)
                                : gridMousePos;

                        var insideRoom = apartment.PointInsideRooms(vertPos, apartment.SelectedRoom);
                        if (insideRoom != null)
                            vertPos = insideRoom.GetNearestPointOnContour(vertPos);

                        apartment.SelectedRoom.MoveVertTo(_SelectedVertIndex, apartment.Dimensions.Clamp(vertPos));
                        break;
                    case SelectionType.Room:
                        apartment.SelectedRoom.Move(-dv);
                        if (!apartment.SelectedRoom.IsInsideRect(dimensions))
                            apartment.SelectedRoom.Move(dv);
                        break;
                }
            }
            _LastMousePosition = mousePositon;
        }
        
        private void DrawSelection()
        {
            var selectedRoom = ApartmentsManager.Instance.CurrentApartment.SelectedRoom;
            if (selectedRoom == null 
                || Selection.activeObject != selectedRoom)
                return;

            switch (_CurrentSelection)
            {
                case SelectionType.Vert:
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(
                        _ParentWindow.Grid.GridToGUI(selectedRoom.GetVertPosition(_SelectedVertIndex)),
                        Vector3.back, Room.SNAPING_RAD / _ParentWindow.Grid.Zoom
                    );
                    break;
                case SelectionType.Room:
                    var walls = selectedRoom.Walls;
                    foreach (Wall wall in walls)
                    {
                        wall.Draw(_ParentWindow.Grid, Color.yellow);
                    }
                    break;
            }
        }

        private void SetupSelection(Vector2 position)
        {
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            switch (_CurrentSelection)
            {
                case SelectionType.Room:
                    apartment.SelectedRoom = apartment.Rooms
                               .FirstOrDefault(x => x.IsPointInside(position));
                    if(apartment.SelectedRoom == null)
                    {
                        _CurrentSelection = SelectionType.None;
                    }
                    break;
                case SelectionType.Vert:
                    _SelectedVertIndex = apartment.SelectedRoom.GetContourVertIndex(position);
                    ApartmentConfig.MakeBackup();
                    ApartmentConfig.Current.IsDrawPositions = true;
                    break;
            }
            if (apartment.SelectedRoom)
            {
                Selection.activeObject = apartment.SelectedRoom;
            }
            else
            {
                Selection.activeObject = apartment;
            }
        }
        private void OnLeftMouse(EventType type, Vector2 mousePosition)
        {
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            switch (type)
            {
                case EventType.MouseDown:
                    _CurrentSelection = SelectionType.None;

                    var mousePos = _ParentWindow.Grid.GUIToGrid(mousePosition);

                    apartment.SelectedRoom = apartment.Rooms
                        .FirstOrDefault(x => x.GetContourVertIndex(mousePos) >= 0);
                   
                    _CurrentSelection = apartment.SelectedRoom == null ? SelectionType.Room : SelectionType.Vert;

                    SetupSelection(mousePos);
                    if (apartment.SelectedRoom)
                        Undo.RegisterCompleteObjectUndo(apartment.SelectedRoom, "Room position changed");
                    _LastMousePosition = mousePosition;
                    break;
                case EventType.MouseDrag:
                    MoveSelected(mousePosition);
                    break;
                case EventType.MouseUp:
                    ApartmentConfig.ApplyBackup();
                    if (apartment.SelectedRoom != null)
                    {
                        apartment.SelectedRoom.RoundContourPoints();
                    }
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