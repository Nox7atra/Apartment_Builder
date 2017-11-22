using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
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
            _CurrentRoom = enable ? Room.Create(ApartmentsManager.Instance.CurrentApartment) : null;
            if (enable)
            {
                Undo.RegisterCreatedObjectUndo(_CurrentRoom, "Room Created");
            }
        }

        public override void Draw()
        {
            if (!_IsActive)
                return;
            var mousePos = _CurrentMousePosition;
            if (_CurrentRoom != null && _CurrentRoom.Walls.Count > 0)
            {
                Handles.color = Color.gray;
                var walls = _CurrentRoom.Walls;
                Handles.DrawLine(
                    mousePos,
                    _ParentWindow.Grid.GridToGUI(walls[walls.Count - 1].End)
                );
            }

            DrawMouseLabel(mousePos);

            _CurrentRoom.Draw(_ParentWindow.Grid);
        }
        #endregion

        #region key events
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;
            switch (type)
            {
                case EventType.MouseMove:
                    _CurrentMousePosition = @event.mousePosition;
                    _ParentWindow.Repaint();
                    break;
                case EventType.MouseDown:
                    var position = _ParentWindow.Grid.GUIToGrid(@event.mousePosition);
                    if (@event.button == 0 && ApartmentsManager.Instance.CurrentApartment.Dimensions.Contains(position))
                    {
                        Undo.RegisterCompleteObjectUndo(_CurrentRoom, "Add Room point");
                        if (!_CurrentRoom.Add(position))
                        {
                            Undo.ClearAll();
                            _ParentWindow.CreateRoomStateEnd(_CurrentRoom);
                        }
                    }
                    break;
                case EventType.KeyDown:
                    if (@event.keyCode == KeyCode.Escape)
                    {
                        Object.DestroyImmediate(_CurrentRoom, true);
                        AssetDatabase.SaveAssets();
                        _ParentWindow.ActivateState(ApartmentEditorWindow.EditorWindowState.Normal);
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
    }
}