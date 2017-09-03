using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Nox7atra.ApartmentEditor
{
    public class Skin : ScriptableObject
    {
        public GUIStyle MiniButtonStyle;
        public Texture IconSave;
        public Texture IconCreateRoom;
        public Texture IconRecenter;
        public void RefreshDefaultStyles()
        {
            MiniButtonStyle = EditorStyles.miniButton;
        }
    }
}