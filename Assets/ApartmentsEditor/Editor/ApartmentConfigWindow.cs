using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nox7atra.ApartmentEditor
{
    public class ApartmentConfigWindow : EditorWindow
    {
        #region factory
        public static ApartmentConfigWindow Create()
        {
            var window = GetWindow<ApartmentConfigWindow>("ApartmentConfig");
            window.Show();
            return window;
        }
        #endregion

        #region attributes
        public static ApartmentDrawConfig Config;

        private static ApartmentDrawConfig? _ConfigBackup;
        #endregion

        #region engine methods
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            Config.IsDrawPositions = GUILayout.Toggle(Config.IsDrawPositions, "Show positions");

            Config.IsDrawSizes = GUILayout.Toggle(Config.IsDrawSizes, "Show sizes");

            Config.IsDrawSquare = GUILayout.Toggle(Config.IsDrawSquare, "Show Squares");
            EditorGUILayout.EndVertical();
        }
        #endregion
        #region public methods
        public static void MakeBackup()
        {
            _ConfigBackup = Config;
        }
        public static void ApplyBackup()
        {
            if (_ConfigBackup.HasValue)
            {
                Config = _ConfigBackup.Value;
                _ConfigBackup = null;
            }
        }
        #endregion

        #region construction
        public ApartmentConfigWindow()
        {
            Config = new ApartmentDrawConfig();
            Config.IsDrawSizes = true;
        }
        #endregion
    }
    public struct ApartmentDrawConfig
    {
        public bool IsDrawSizes;
        public bool IsDrawPositions;
        public bool IsDrawSquare;
    }
}