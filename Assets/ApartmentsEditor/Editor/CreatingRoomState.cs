using System.Collections;
using System.Collections.Generic;
using Nox7atra.ApartmentEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class CreatingRoomState : StateApartmentBuilder
    {
        protected override void OnKeyEvent(EventType type, Event @event)
        {
            if (!_IsActive)
                return;
            throw new System.NotImplementedException();
        }
        public CreatingRoomState(ApartmentEditorWindow parentWindow) : base(parentWindow)
        {
        }
    }
}