using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentEditor
{
    public class ObjectsManager : ScriptableObject
    {
        #region fields

        private Mode _CurrentMode;
        private WallObject _SelectedObject;
        private string[] _Choices;
        #endregion

        #region properties

        public Mode CurrentMode
        {
            get { return _CurrentMode; }
        }

        public string[] Choices
        {
            get { return _Choices; }
        }
        public WallObject SelectedObject
        {
            get { return _SelectedObject; }
        }
        #endregion

        #region public methods

        public void SelectMode(Mode mode)
        {
            _CurrentMode = mode;

            if(_CurrentMode == null)
                return;

            _Choices = GetChoices();
        }

        public void SelectObjectAtChoice(int chioceIndex)
        {
            _SelectedObject = AssetDatabase.LoadAssetAtPath<WallObject>(_Choices[chioceIndex]);
        }
        #endregion

        #region singletone

        private static ObjectsManager _Instance;

        public static ObjectsManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    var path = Path.Combine(PathsConfig.Instance.PathToData, AssetName);
                    _Instance = AssetDatabase.LoadAssetAtPath<ObjectsManager>(path);

                    if (_Instance == null)
                    {
                        _Instance = CreateInstance<ObjectsManager>();
                        AssetDatabase.CreateAsset(_Instance, path);
                    }
                }

                return _Instance;
            }
        }

        #endregion

        #region service methods


        private string[] GetChoices()
        {
            switch (_CurrentMode)
            {
                case Mode.Doors:
                    return Directory.GetFiles(PathsConfig.Instance.PathToDoors, "*.asset");
                case Mode.Windows:
                    return Directory.GetFiles(PathsConfig.Instance.PathToWindows, "*.asset");
            }
            return null;
        }

        
        #endregion
        private const string AssetName = "ObjectsManager.asset";
        public enum Mode
        {
            Doors,
            Windows,
            None
        }
    }
}