using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;

public abstract class StateApartmentEditor
{
    protected bool _IsActive;
    protected StateApartmentEditor(ApartmentEditorWindow parentWindow)
    {
        _parentWindow = parentWindow;
        _parentWindow.onKeyEvent += OnKeyEvent;
        _IsActive = false;
    }
    protected ApartmentEditorWindow _parentWindow;

    public void MouseCallbacks(bool enable)
    {
        _IsActive = enable;
    }

    public void Destroy()
    {
        _parentWindow.onKeyEvent -= OnKeyEvent;
    }
    protected abstract void OnKeyEvent(EventType type);
    
}
