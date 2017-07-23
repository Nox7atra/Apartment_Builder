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
        private bool _IsDrawCenter;
        #endregion

        #region public methods
        public Vector2 GUIToGrid(Vector3 vec)
        {
            return (new Vector2(
                       vec.x + _ParentWindow.position.width / 2,
                       -vec.y - _ParentWindow.position.height / 2) - _Offset) ;
        }
        public void Draw()
        {
            DrawCenter();
            DrawLines();
        }
        public void Recenter()
        {
            _Offset = Vector2.zero;
        }
        public void Move(Vector3 dv)
        {
            var x = _Offset.x + dv.x;
            var y = _Offset.y + dv.y;
            float halfLength = DEFAULT_CELL_SIZE * CELLS_IN_LINE_COUNT;
            _Offset.x = x > halfLength ? halfLength : x < -halfLength ? -halfLength : x;
            _Offset.y = y > halfLength ? halfLength : y < -halfLength ? -halfLength : y;
        }
        #endregion

        #region service methods
        void DrawLines()
        {
            DrawLODLines(0);
        }
        void DrawLODLines(int level)
        {
            var step0 = Mathf.Pow(10, level);
            int halfCount = (int) step0 * CELLS_IN_LINE_COUNT / 2 * 10;
            var length = halfCount * DEFAULT_CELL_SIZE;
            for (int i = -halfCount; i <= halfCount; i += (int)step0)
            {
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b,  step0 / 4);
                
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
                Handles.color = new Color(GRID_COLOR.r, GRID_COLOR.g, GRID_COLOR.b,  step0);
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
        #endregion

        #region constructor

        public Grid(EditorWindow parentWindow, bool isDrawCenterMark = false)
        {
            _ParentWindow = parentWindow;
            _IsDrawCenter = isDrawCenterMark;
            Recenter();
        }

        #endregion

        #region constants
        const int   CELLS_IN_LINE_COUNT        = 40;
        const float DEFAULT_CELL_SIZE          = 20;
        private const float ZOOM_SPEED         = 0.05f;
        private static readonly Color GRID_COLOR = new Color(0.3f, 0.3f, 0.3f);
        #endregion
    }
}
