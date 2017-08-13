using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class Toolbar
    {
        #region attributes
        ApartmentEditorWindow _ParentWindow;
        #endregion

        #region public methods
        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            CreateRoomButton();
            RecenterButton();
            SaveButton();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region buttons
        public void CreateRoomButton()
        {
            if (GUILayout.Button("CreateRoom"))
            {
                _ParentWindow.CreateRoomBegin();
            }
        }
        public void SaveButton()
        {
            if (GUILayout.Button("Save" + (_ParentWindow.ApartmentManager.NeedToSave ? "*" : "")))
            {
                _ParentWindow.ApartmentManager.SaveCurrent();
            }
        }
        public void RecenterButton()
        {
            if (GUILayout.Button("Recenter"))
            {
                _ParentWindow.Grid.Recenter();
            }
        }
        #endregion
        #region contstruction
        public Toolbar(ApartmentEditorWindow parentWindow)
        {
            _ParentWindow = parentWindow;
        }
        #endregion

    }
}