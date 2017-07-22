using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Grid
    {
        #region properties
        public bool IsDrawCenter
        {
            get
            {
                return _IsDrawCenter;
            }
            set
            {
                _IsDrawCenter = value;
            }
        }
        #endregion
        #region attributes
        readonly EditorWindow _ParentWindow;

        private Vector2 _Offset;
        private float _Zoom;
        private Matrix4x4 _OriginalGUIMatrix;
        private int _CurrentLodLevel;
        private bool _IsDrawCenter;
        #endregion

        #region public methods
        public Vector2 GUIToGrid(Vector3 vec)
        {
            return new Vector2(
                       vec.x + _ParentWindow.position.width / 2,
                       vec.y + _ParentWindow.position.height / 2) - _Offset;
        }
        public void Draw()
        {
            _OriginalGUIMatrix = Handles.matrix;
            Zooming();
            DrawCenter();
            DrawLines();
            Handles.matrix = _OriginalGUIMatrix;
        }
        public void Recenter()
        {
            _Offset = Vector2.zero;
        }
        public void Move(Vector3 dv)
        {
            var x = _Offset.x + dv.x / _Zoom;
            var y = _Offset.y + dv.y / _Zoom;
            float halfLength = DEFAULT_CELL_SIZE * CELLS_IN_LINE_COUNT * MAX_LOD_LEVEL;
            _Offset.x = x > halfLength ? halfLength : x < -halfLength ? -halfLength : x;
            _Offset.y = y > halfLength ? halfLength : y < -halfLength ? -halfLength : y;
        }
        public void Zoom(float delta)
        {
            var zoom = _Zoom - delta * _Zoom * ZOOM_SPEED;
            _Zoom = zoom <= MIN_ZOOM ? MIN_ZOOM : zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
            _CurrentLodLevel = CalculateLodLevel();
        }
        #endregion

        #region service methods
        void DrawLines()
        {
            DrawLODLines(_CurrentLodLevel);
        }
        void DrawLODLines(int level)
        {
            var step0 = Mathf.Pow(10, level);
            int halfCount = (int) step0 * CELLS_IN_LINE_COUNT / 2 * 10;
            var length = halfCount * DEFAULT_CELL_SIZE;
            for (int i = -halfCount; i <= halfCount; i += (int)step0)
            {
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b, _Zoom * step0 / 4);
                
                Handles.DrawLine(
                    GUIToGrid(new Vector2(-length, i * DEFAULT_CELL_SIZE)),
                    GUIToGrid(new Vector2(length, i * DEFAULT_CELL_SIZE))
                );
                Handles.DrawLine(
                    GUIToGrid(new Vector2(i * DEFAULT_CELL_SIZE, -length)),
                    GUIToGrid(new Vector2(i * DEFAULT_CELL_SIZE, length))
                );
            }
            for (int i = -halfCount; i <= halfCount; i += (int)step0 * 10)
            {
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b, _Zoom * step0);
                Handles.DrawLine(
                    GUIToGrid(new Vector2(-length, i * DEFAULT_CELL_SIZE)),
                    GUIToGrid(new Vector2(length, i * DEFAULT_CELL_SIZE))
                );
                Handles.DrawLine(
                    GUIToGrid(new Vector2(i * DEFAULT_CELL_SIZE, -length)),
                    GUIToGrid(new Vector2(i * DEFAULT_CELL_SIZE, length))
                );
            }
        }
        void DrawCenter()
        {
            if (!_IsDrawCenter)
                return;

            Handles.color = Color.cyan;
            Handles.DrawLine(GUIToGrid(Vector3.left * DEFAULT_CELL_SIZE), 
                GUIToGrid(Vector3.right * DEFAULT_CELL_SIZE));
            Handles.DrawLine(GUIToGrid(Vector3.down * DEFAULT_CELL_SIZE),
                GUIToGrid(Vector3.up * DEFAULT_CELL_SIZE));
        }
        void Zooming()
        {
            var Translation = Matrix4x4.TRS(
                new Vector3(_ParentWindow.position.width / 2, _ParentWindow.position.height / 2), 
                Quaternion.identity,
                Vector3.one
            );
            Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(_Zoom, _Zoom, 1.0f));
            Handles.matrix = Translation * Scale * Translation.inverse;
        }

        int CalculateLodLevel()
        {
            return -(int)Mathf.Log10(_Zoom * 0.5f);
        }
        float CalculateZoomFromLod(int level)
        {
            return Mathf.Pow(10, -level) * 2;
        }
        #endregion

        #region constructor

        public Grid(EditorWindow parentWindow, bool isDrawCenterMark = false)
        {
            _ParentWindow = parentWindow;
            _Zoom = CalculateZoomFromLod(DEFAULT_LOD_LEVEL);
            _CurrentLodLevel = CalculateLodLevel();
            _IsDrawCenter = isDrawCenterMark;
            Recenter();
        }

        #endregion

        #region constants
        const int   CELLS_IN_LINE_COUNT        = 40;
        const float DEFAULT_CELL_SIZE          = 20;
        private const float MAX_ZOOM           = 1;
        private const float MIN_ZOOM           = 0.01f;
        private const int DEFAULT_LOD_LEVEL = 1;
        private const int MAX_LOD_LEVEL = 2;
        private const float ZOOM_SPEED         = 0.05f;
        private static readonly Color GRID_COLOR = new Color(0.3f, 0.3f, 0.3f);
        #endregion
    }
}
