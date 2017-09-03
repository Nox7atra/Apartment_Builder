using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Nox7atra.ApartmentEditor
{
    public class Skin : ScriptableObject
    {
        public GUIStyle ToolbarMiniButtonStyle;
        public Texture ToolbarIconSave;
        public Texture ToolbarIconCreateRoom;

        private void OnEnable()
        {
            
        }
        
    }
}