using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentEditor
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
        public Color KitchenContourColor;
        public Color BathroomContourColor;
        public Color ToiletContourColor;
        public Color LivingContourColor;
        public Color RoomDimensionsColor;
        public Color SelectionColor;
        public Color DoorColor;
        public Color WindowColor;
        public void RefreshDefaultStyles()
        {
            MiniButtonStyle = EditorGUIUtility.isProSkin ? EditorStyles.miniButton : EditorStyles.miniButtonLeft;
        }

        public Color GetColor(Room.Type type)
        {
            switch (type)
            {
                default:
                    return Color.magenta;
                case Room.Type.Bathroom:
                    return BathroomContourColor;
                case Room.Type.Kitchen:
                    return KitchenContourColor;
                case Room.Type.Living:
                    return LivingContourColor;
                case Room.Type.Toilet:
                    return ToiletContourColor;
            }
        }
    }
}