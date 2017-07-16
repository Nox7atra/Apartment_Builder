using System;
using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;

public class NormalStateApartmentEditor : StateApartmentEditor
{
    #region attributes
    private Vector3? _lastMousePosition;
    #endregion

    #region service methods

    void DragGrid()
    {
        var curMousePosition = Event.current.mousePosition;
        if (_lastMousePosition.HasValue)
        { 
            var dv = GUIUtility.GUIToScreenPoint( (Vector2) _lastMousePosition)
                - GUIUtility.GUIToScreenPoint(curMousePosition);
            _parentWindow.Grid.Move(dv);
            _parentWindow.Repaint();
        }
        _lastMousePosition = curMousePosition;
    }

    #endregion
    #region events
    protected override void OnKeyEvent(EventType type)
    {
        if(!_IsActive)
            return;

        switch (type)
        {
            case EventType.MouseDrag:
                DragGrid();
                break;
            case EventType.MouseDown:
                _lastMousePosition = null;
                break;
        }
    }

    #endregion

    #region constructors
    public NormalStateApartmentEditor(ApartmentEditorWindow parentWindow) : base(parentWindow)
    {
    }
    #endregion
}
