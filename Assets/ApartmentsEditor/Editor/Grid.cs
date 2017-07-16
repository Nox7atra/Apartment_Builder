using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Grid
    {
        #region properties

        public int CurrentLodLevel
        {
            get { return -(int) Mathf.Log10(_zoom * 0.5f); }
        }

        #endregion
        #region attributes
        readonly EditorWindow _parentWindow;

        Vector2      _offset;
        private float _zoom;
        private Matrix4x4 _OriginalGUIMatrix;

        #endregion

        #region public methods
        public Vector2 GUIToGrid(Vector3 vec)
        {
            return new Vector2(
                       vec.x + _parentWindow.position.width / 2,
                       vec.y + _parentWindow.position.height / 2) - _offset;
        }
        public void Draw()
        {
            _OriginalGUIMatrix = Handles.matrix;
            Zooming();
            Handles.BeginGUI();
            DrawLines();
            Handles.EndGUI();
            Handles.matrix = _OriginalGUIMatrix;
        }
        public void Recenter()
        {
            _offset = Vector2.zero;
        }
        public void Move(Vector3 dv)
        {
            var x = _offset.x + dv.x / _zoom;
            var y = _offset.y + dv.y / _zoom;
            var gridHalfLength = (CurrentLodLevel + 1) * DEFAULT_CELL_SIZE * CELLS_IN_LINE_COUNT / 2;
            _offset.x = x;
            _offset.y = y;
        }

        public void Zoom(float delta)
        {
            var zoom = _zoom - delta * _zoom * ZOOM_SPEED;
            _zoom = zoom <= MIN_ZOOM ? MIN_ZOOM : zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
        }
        #endregion

        #region service methods
        void DrawLines()
        {
            DrawLODLines(CurrentLodLevel);
        }
        void DrawLODLines(int level)
        {
            var step0 = Mathf.Pow(10, level);
            int halfCount = (int) step0 * CELLS_IN_LINE_COUNT / 2 * 10;
            var length = halfCount * DEFAULT_CELL_SIZE;
            for (int i = -halfCount; i <= halfCount; i += (int)step0)
            {
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b, _zoom * step0 / 4);
                
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
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b, _zoom * step0);
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

        void Zooming()
        {
            var Translation = Matrix4x4.TRS(
                new Vector3(_parentWindow.position.width/2, _parentWindow.position.height/2), 
                Quaternion.identity,
                Vector3.one
            );
            Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(_zoom, _zoom, 1.0f));
            Handles.matrix = Translation * Scale * Translation.inverse;
        }
        #endregion

        #region constructors

        public Grid(EditorWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _zoom = 0.1f;
            Recenter();
        }

        #endregion

        #region constants
        const int   CELLS_IN_LINE_COUNT        = 30;
        const float CENTER_SIZE_IN_CELLS       = 0.1f;
        const float DEFAULT_CELL_SIZE          = 20;
        private const float MAX_ZOOM           = 1;
        private const float MIN_ZOOM           = 0.01f;
        private const float ZOOM_SPEED         = 0.05f;
        private static readonly Color GRID_COLOR = new Color(0.3f, 0.3f, 0.3f);
        #endregion
    }
}
