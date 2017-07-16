using UnityEditor;
using UnityEngine;

public class Grid
{
    #region constants
    const           float   DEFAULT_CELL_SIZE = 15;
    static readonly Color   GRID_COLOR        = new Color(0.5f,0.5f, 0.5f);
    static readonly Vector3 DEFAULT_OFFSET    = new Vector3(DEFAULT_CELL_SIZE / 2, DEFAULT_CELL_SIZE / 2);
    #endregion

    #region attributes
    Vector3 m_Offset;
    float m_Zoom;

    #endregion

    #region public methods

    public void Draw(Vector2 windowSize)
    {
        Handles.BeginGUI();
        Handles.color = Color.gray;
        for (int i = 0; i < (int)(windowSize.y / DEFAULT_CELL_SIZE) + 1; i++)
        {
            Handles.DrawLine(
                new Vector3(0, i * DEFAULT_CELL_SIZE, 0) + m_Offset,
                new Vector3(windowSize.y * DEFAULT_CELL_SIZE, i * DEFAULT_CELL_SIZE) + m_Offset
            );
        }
        for (int i = 0; i < (int)(windowSize.x / DEFAULT_CELL_SIZE) + 1; i++)
        {
            Handles.DrawLine(
                new Vector3(i * DEFAULT_CELL_SIZE, 0, 0) + m_Offset, 
                new Vector3(i * DEFAULT_CELL_SIZE, windowSize.x * DEFAULT_CELL_SIZE) + m_Offset
            );
        }
        Handles.EndGUI();
    }
    #endregion

    #region constructors

    public Grid()
    {
        m_Offset = DEFAULT_OFFSET;
        m_Zoom = 1f;
    }

    #endregion
}
