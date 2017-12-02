using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class AddObjectState : StateApartmentBuilder
    {
        #region fields
        
        private Vector2 _CurrentMousePosition;

        #endregion

        #region activation

        public override void SetActive(bool enable)
        {
            base.SetActive(enable);
            
            if (!enable)
            {
                return;
            }
            
        }

        #endregion

        #region service methods



        #endregion

        #region drawing
        public override void Draw()
        {
            if (!_IsActive)
                return;
            var apartment = ApartmentsManager.Instance.CurrentApartment;
            var objgridPos = apartment.GetPointProjectionOnNearestContour(_ParentWindow.Grid.GUIToGrid(_CurrentMousePosition));
            var walls = apartment.GetWallsWithPoint(objgridPos);
            foreach (var wall in walls)
            {
                wall.DrawWallObject(_ParentWindow.Grid, ObjectsManager.Instance.SelectedObject, wall.GetPositionWallObjectFromPoint(objgridPos));
            }
        }


        #endregion
       
        #region key events

        protected override void OnKeyEvent(EventType type, Vector2 mousePosition, KeyCode code)
        {
            if (!_IsActive)
                return;
            switch (type)
            {
                case EventType.MouseMove:
                    _CurrentMousePosition = mousePosition;
                    _ParentWindow.Repaint();
                    break;

                case EventType.KeyDown:
                    if (code == KeyCode.Escape)
                    {
                        AssetDatabase.SaveAssets();
                        _ParentWindow.ActivateState(ApartmentEditorWindow.EditorWindowState.Normal);
                        Selection.activeObject = ApartmentsManager.Instance.CurrentApartment;
                    }

                    break;
            }
        }

        #endregion

        #region construcor
        public AddObjectState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }
#endregion
        
    }
}