using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentEditor
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
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            _CurrentRoom = enable ? Room.Create(apartment) : null;
            
            if (enable)
            {
                if(apartment.Rooms.Count > 0)
                    _CurrentRoom.WallThickness = apartment.Rooms[apartment.Rooms.Count - 1].WallThickness;
                Undo.RegisterCreatedObjectUndo(_CurrentRoom, "Room Created");
            }
        }

        public override void Draw()
        {
            if (!_IsActive)
                return;
            var mousePos = _CurrentMousePosition;
            if (_CurrentRoom != null && _CurrentRoom.Contour.Count > 0)
            {
                Handles.color = Color.gray;
                var contour = _CurrentRoom.Contour;
                Handles.DrawLine(
                    mousePos,
                    _ParentWindow.Grid.GridToGUI(contour[contour.Count - 1].Position));
            }

            DrawMouseLabel(mousePos);

            _CurrentRoom.Draw(_ParentWindow.Grid, false);
        }
        #endregion

        #region key events
        protected override void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode keyCode)
        {
            if (!_IsActive)
                return;
            base.OnKeyEvent(type, mousePosition, keyCode);
            switch (type)
            {
                case EventType.MouseMove:
                    _CurrentMousePosition = mousePosition;
                    _ParentWindow.Repaint();
                    break;
                case EventType.MouseDown:
                    var position = _ParentWindow.Grid.GUIToGrid(mousePosition);
                    if (Event.current.button == 0)
                    {
                        Undo.RegisterCompleteObjectUndo(_CurrentRoom, "Add Room point");
                        if (!_CurrentRoom.Add(_CurrentRoom.ParentApartment.Dimensions.Clamp(position)))
                        {
                            Undo.ClearAll();
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

        protected override void Reset()
        {
            Object.DestroyImmediate(_CurrentRoom, true);
            AssetDatabase.SaveAssets();
            _ParentWindow.ActivateState(ApartmentEditorWindow.EditorWindowState.Normal);
        }
    }
}