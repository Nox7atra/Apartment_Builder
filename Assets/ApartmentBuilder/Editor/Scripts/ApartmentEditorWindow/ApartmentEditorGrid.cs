using System;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    public class ApartmentBuilderGrid
    {
        public float Zoom
        {
            get
            {
                return _Zoom;
            }
            set
            {
                _Zoom = value > MAX_ZOOM_VALUE ? MAX_ZOOM_VALUE : value < MIN_ZOOM_VALUE ? MIN_ZOOM_VALUE : value;
            }
        }
        public bool IsDrawCenter;

        private readonly EditorWindow _ParentWindow;

        private Vector2 _Offset;
        private float _Zoom;
 

        #region public methods
        public Vector2 GUIToGrid(Vector3 vec)
        {
            Vector2 newVec = (
                new Vector2(vec.x, -vec.y) - new Vector2(_ParentWindow.position.width / 2, -_ParentWindow.position.height / 2)) 
                * _Zoom + new Vector2(_Offset.x, -_Offset.y);
            return newVec.RoundCoordsToInt();
        }
        public Vector2 GridToGUI(Vector3 vec)
        {
            return (new Vector2(vec.x - _Offset.x, -vec.y - _Offset.y) ) / _Zoom 
                + new Vector2(_ParentWindow.position.width / 2, _ParentWindow.position.height / 2);
        }
        public void Draw()
        {
            DrawLines();
            DrawCenter();
        }
        public void Recenter()
        {
            _Offset = Vector3.zero;
        }
        public void Move(Vector3 dv)
        {
            var x = _Offset.x + dv.x * _Zoom;
            var y = _Offset.y + dv.y * _Zoom;
            _Offset.x = x;
            _Offset.y = y;
        }
        #endregion

        #region service methods
        void DrawLines()
        {
            int lodLevel = (int) (Mathf.Log(_Zoom) / 1.5f);
            DrawLODLines(lodLevel > 0 ? lodLevel : 0);
        }
        void DrawLODLines(int level)
        {
            var gridColor = SkinManager.Instance.CurrentSkin.GridColor;
            var step0 = (int) Mathf.Pow(10, level);
            int halfCount = step0 * CELLS_IN_LINE_COUNT / 2 * 10;
            var length = halfCount * DEFAULT_CELL_SIZE;
            int offsetX = ((int) (_Offset.x / DEFAULT_CELL_SIZE)) / (step0 * step0) * step0;
            int offsetY = ((int) (_Offset.y / DEFAULT_CELL_SIZE)) / (step0 * step0) * step0;
            for (int i = -halfCount; i <= halfCount; i += step0)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b,  0.3f);
                    
                Handles.DrawLine(
                    GridToGUI(new Vector2(-length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2(length  + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE))
                );
                Handles.DrawLine(
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, -length + offsetY * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, length + offsetY * DEFAULT_CELL_SIZE))
                );
            }
            offsetX = (offsetX / (10 * step0)) * 10 * step0;
            offsetY = (offsetY / (10 * step0)) * 10 * step0; ;
            for (int i = -halfCount; i <= halfCount; i += step0 * 10)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b,  1);
                Handles.DrawLine(
                    GridToGUI(new Vector2(-length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2(length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE))
                );
                Handles.DrawLine(
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, -length + offsetY * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, length + offsetY * DEFAULT_CELL_SIZE))
                );
            }
        }
        void DrawCenter()
        {
            if (!IsDrawCenter)
                return;

            Handles.color = Color.cyan;
            Handles.DrawLine(GridToGUI(Vector3.left * DEFAULT_CELL_SIZE * _Zoom),
                GridToGUI(Vector3.right * DEFAULT_CELL_SIZE * _Zoom));
            Handles.DrawLine(GridToGUI(Vector3.down * DEFAULT_CELL_SIZE * _Zoom),
                GridToGUI(Vector3.up * DEFAULT_CELL_SIZE * _Zoom));
        }
        #endregion

        #region constructor

        public ApartmentBuilderGrid(EditorWindow parentWindow, bool isDrawCenterMark = true)
        {
            _Zoom = 5f;
            _ParentWindow = parentWindow;
            IsDrawCenter = isDrawCenterMark;
            Recenter();
        }

        #endregion

        #region constants
        const float MIN_ZOOM_VALUE             = 0.1f;
        const float MAX_ZOOM_VALUE             = 1000;
        const int   CELLS_IN_LINE_COUNT        = 60;
        const float DEFAULT_CELL_SIZE          = 10;
        #endregion
    }
}
