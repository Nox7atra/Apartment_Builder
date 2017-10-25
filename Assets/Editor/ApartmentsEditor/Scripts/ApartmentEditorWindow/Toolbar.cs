using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Toolbar
    {
        private readonly ApartmentEditorWindow _ParentWindow;
        private readonly Skin _CurrentSkin;
        public void Draw()
        {
            GUILayout.BeginArea(new Rect(0,0, Screen.width / 2f, Screen.height));
            EditorGUILayout.BeginVertical();
            RecenterButton();
            CreateRoomButton();
            SaveButton();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void CreateRoomButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                    _CurrentSkin.IconCreateRoom,
                    "Create Room",
                    _CurrentSkin.MiniButtonStyle
                ))
            {
                _ParentWindow.CreateRoomStateBegin();
            }
        }
        public void SaveButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                    _CurrentSkin.IconSave, 
                    "Save", 
                    _CurrentSkin.MiniButtonStyle
                ))
            {
                //TODO: Make implementation 
            }
        }
        public void RecenterButton()
        {
            if (EditorUIUtils.ButtonWithFallback(
                    _CurrentSkin.IconRecenter,
                    "Recenter",
                    _CurrentSkin.MiniButtonStyle
                ))
            {
                _ParentWindow.Grid.Recenter();
            }
        }
        public Toolbar(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
            _CurrentSkin = SkinManager.Instance.CurrentSkin;
        }
    }
}