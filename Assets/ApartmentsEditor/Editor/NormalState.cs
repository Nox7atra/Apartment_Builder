using System;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class NormalState : StateApartmentBuilder
    {
        #region public methods
        public override void Draw()
        {
            if (!_IsActive)
                return;
        }
        #endregion
        #region events
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;  
        }
        #endregion


        #region constructors

        public NormalState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }

        #endregion
    }
}