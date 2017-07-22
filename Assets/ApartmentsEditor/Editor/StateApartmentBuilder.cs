using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public abstract class StateApartmentBuilder
    {

        protected StateApartmentBuilder(ApartmentEditorWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _parentWindow.onKeyEvent += OnKeyEvent;
            _IsActive = false;
        }

        protected bool _IsActive;
        protected ApartmentEditorWindow _parentWindow;

        public void MouseCallbacks(bool enable)
        {
            _IsActive = enable;
        }

        public void Destroy()
        {
            _parentWindow.onKeyEvent -= OnKeyEvent;
        }

        protected abstract void OnKeyEvent(EventType type, Event @event);
    }
}