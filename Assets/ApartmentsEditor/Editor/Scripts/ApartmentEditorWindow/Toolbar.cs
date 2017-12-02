using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
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
            if (EditorUIUtils.ButtonWithFallback(
                null,
                "ad",
                _CurrentSkin.MiniButtonStyle
            ))
            { 
                _ParentWindow.AddObjectStateBegin(ObjectsManager.Mode.Doors);
            }
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