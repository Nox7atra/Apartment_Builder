using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Foxsys.ApartmentBuilder
{
    public abstract class StateHotkeys
    {
        #region properties

        public bool IsProjectionHotkeyPressed{get { return _IsProjectionHotkeyPressed; }}
        #endregion
        private bool _IsProjectionHotkeyPressed;
        public virtual void Use(KeyCode keyCode, bool isKeyDown)
        {
            switch (keyCode)
            {
                case KeyCode.A:
                    _IsProjectionHotkeyPressed = isKeyDown;
                    break;
            }
        }
    }
}