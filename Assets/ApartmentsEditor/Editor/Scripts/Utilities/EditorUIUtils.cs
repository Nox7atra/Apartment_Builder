using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public static class EditorUIUtils
    {
        public static bool ButtonWithFallback(Texture icon, string text, GUIStyle style)
        {
            return icon != null
                ? GUILayout.Button(icon, style)
                : GUILayout.Button(text, style);
        }
    }
}