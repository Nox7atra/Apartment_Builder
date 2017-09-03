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
            GUILayout.BeginArea(new Rect(0,0, Screen.width / 2, Screen.height));
            EditorGUILayout.BeginVertical();
            CreateRoomButton();
            RecenterButton();
            SaveButton();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void CreateRoomButton()
        {
            if (GUILayout.Button(
                "CR", 
                SkinManager.Instance.CurrentSkin.ToolbarMiniButtonStyle))
            {
                _ParentWindow.CreateRoomStateBegin();
            }
        }
        public void SaveButton()
        {
    
            if (GUILayout.Button(
                _CurrentSkin.ToolbarIconSave,
                _CurrentSkin.ToolbarMiniButtonStyle))
            {
                _ParentWindow.ApartmentManager.SaveCurrent();
            }
        }
        public void RecenterButton()
        {
            if (GUILayout.Button(
                "R",
                SkinManager.Instance.CurrentSkin.ToolbarMiniButtonStyle))
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