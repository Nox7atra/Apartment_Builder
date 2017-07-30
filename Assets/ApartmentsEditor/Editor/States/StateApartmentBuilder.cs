using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public abstract class StateApartmentBuilder
    {
        #region attributes
        protected bool _IsActive;
        protected ApartmentEditorWindow _ParentWindow;
        #endregion
        public virtual void SetActive(bool enable)
        {
            _IsActive = enable;
        }

        public void Destroy()
        {
            _ParentWindow.onKeyEvent -= OnKeyEvent;
        }
        protected abstract void OnKeyEvent(EventType type, Event @event);
        public abstract void Draw();
        protected StateApartmentBuilder(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _ParentWindow.onKeyEvent += OnKeyEvent;
            _IsActive = false;
        }
    }
}