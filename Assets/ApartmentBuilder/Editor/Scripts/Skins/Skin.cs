using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentBuilder
{
    public class Skin : ScriptableObject
    {
        public GUIStyle MiniButtonStyle;
        public GUIStyle TextStyle;
        public Texture IconSave;
        public Texture IconCreateRoom;
        public Texture IconRecenter;
        public Color GridColor;
        public Color VertColor;
        public Color RoomDimensionsColor;
        public Color SelectionColor;
        public Color DoorColor;
        public Color WindowColor;
        public float CirclesRad;
        public void RefreshDefaultStyles()
        {
            MiniButtonStyle = EditorGUIUtility.isProSkin ? EditorStyles.miniButton : EditorStyles.miniButtonLeft;
        }
    }
}