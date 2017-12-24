using System;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class ApartmentEditorGrid
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
            Vector2 newVec = ((Vector2)vec - new Vector2(_ParentWindow.position.width / 2, _ParentWindow.position.height / 2)) * _Zoom + _Offset;
            return newVec.RoundCoordsToInt();
        }
        public Vector2 GridToGUI(Vector3 vec)
        {
            return ((Vector2) vec  - _Offset) / _Zoom + new Vector2(_ParentWindow.position.width / 2, _ParentWindow.position.height / 2);
        }
        public void Draw()
        {
            DrawCenter();
            DrawLines();
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
            //float halfLength = DEFAULT_CELL_SIZE * CELLS_IN_LINE_COUNT;
            //_Offset.x = x > halfLength ? halfLength : x < -halfLength ? -halfLength : x;
            //_Offset.y = y > halfLength ? halfLength : y < -halfLength ? -halfLength : y;
        }
        #endregion

        #region service methods
        void DrawLines()
        {
            DrawLODLines(0);
        }
        void DrawLODLines(int level)
        {
            var gridColor = SkinManager.Instance.CurrentSkin.GridColor;
            var step0 = Mathf.Pow(10, level);
            int halfCount = (int) step0 * CELLS_IN_LINE_COUNT / 2 * 10;
            var length = halfCount * DEFAULT_CELL_SIZE;
            int offsetX = (int) (_Offset.x / DEFAULT_CELL_SIZE);
            int offsetY = (int) (_Offset.y / DEFAULT_CELL_SIZE);
            for (int i = -halfCount; i <= halfCount; i += (int)step0)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b,  step0 / 4);
                    
                Handles.DrawLine(
                    GridToGUI(new Vector2(-length + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2(length  + offsetX * DEFAULT_CELL_SIZE, (i + offsetY) * DEFAULT_CELL_SIZE))
                );
                Handles.DrawLine(
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, -length + offsetY * DEFAULT_CELL_SIZE)),
                    GridToGUI(new Vector2((i + offsetX) * DEFAULT_CELL_SIZE, length + offsetY * DEFAULT_CELL_SIZE))
                );
            }
            offsetX = (offsetX / 10) * 10;
            offsetY = (offsetY / 10) * 10;
            for (int i = -halfCount; i <= halfCount; i += (int)step0 * 10)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b,  step0);
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
            Handles.DrawLine(GridToGUI(Vector3.left * DEFAULT_CELL_SIZE),
                GridToGUI(Vector3.right * DEFAULT_CELL_SIZE));
            Handles.DrawLine(GridToGUI(Vector3.down * DEFAULT_CELL_SIZE),
                GridToGUI(Vector3.up * DEFAULT_CELL_SIZE));
        }
        #endregion

        #region constructor

        public ApartmentEditorGrid(EditorWindow parentWindow, bool isDrawCenterMark = true)
        {
            _Zoom = 0.9f;
            _ParentWindow = parentWindow;
            IsDrawCenter = isDrawCenterMark;
            Recenter();
        }

        #endregion

        #region constants
        const float MIN_ZOOM_VALUE             = 0.2f;
        const float MAX_ZOOM_VALUE             = 8;
        const int   CELLS_IN_LINE_COUNT        = 70;
        const float DEFAULT_CELL_SIZE          = 20;
        #endregion
    }
}
