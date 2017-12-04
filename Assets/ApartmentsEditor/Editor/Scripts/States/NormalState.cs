using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Foxsys.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region fields

        private ISelectable _SelectedObject;
        #endregion

        #region activation

        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            _SelectedObject = null;
        }

        #endregion

        #region service methods

        public void DeleteSelected()
        {
            if (_SelectedObject == null)
                return;

            _SelectedObject.Delete();
            _SelectedObject = null;
            AssetDatabase.SaveAssets();
        }

        private void MoveSelected(Vector2 mousePositon)
        {
            if (_SelectedObject == null)
                return;
            _SelectedObject.MoveTo(mousePositon);
        
        }

        #endregion

        #region drawing

        public override void Draw()
        {
            if (!_IsActive)
                return;
            DrawSelection();
        }

        private void DrawSelection()
        {
            if (_SelectedObject == null)
                return;
            var color = SkinManager.Instance.CurrentSkin.SelectionColor;
            _SelectedObject.DrawSelection(_ParentWindow.Grid, color);
        }

        #endregion


        #region key events

        protected override void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode keyCode)
        {
            if (!_IsActive)
                return;

            if (Event.current.button == 0)
            {
                OnLeftMouse(type, mousePosition);
            }

            if (type == EventType.KeyDown)
            {
                _Hotkeys.Use(keyCode, true);
            }
            if (type == EventType.KeyUp)
            {
                _Hotkeys.Use(keyCode, false);
            }
            _ParentWindow.Repaint();
        }
        private void OnLeftMouse(EventType type, Vector2 mousePosition)
        {
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            var mousePos = _ParentWindow.Grid.GUIToGrid(mousePosition);
            switch (type)
            {
                case EventType.MouseDown:
                    _SelectedObject = null;
                    Room selectedRoom;
                    var vert = apartment.GetVertInPos(mousePos, out selectedRoom);

                    selectedRoom = selectedRoom == null
                        ?  apartment.Rooms
                            .FirstOrDefault(x => x.IsPointInside(mousePos))
                        : selectedRoom;
                    if (selectedRoom != null)
                    {

                        Selection.activeObject = selectedRoom;
                        Undo.RegisterCompleteObjectUndo(selectedRoom, "Room position changed");
                        selectedRoom.IsShowPositions = true;
                       
                        _SelectedObject = (vert != null ? (ISelectable) vert : selectedRoom );
                    }
                    else
                    {
                        Selection.activeObject = apartment;
                    }
                    
                    break;
                case EventType.MouseDrag:
                    MoveSelected(mousePos);
                    break;
                case EventType.MouseUp:
                    if (_SelectedObject != null)
                    {
                        _SelectedObject.EndMoving();
                    }
                    break;
            }
        }

        #endregion

        public NormalState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
            _Hotkeys = new NormalStateHotkeys(this);
        }
    }
}