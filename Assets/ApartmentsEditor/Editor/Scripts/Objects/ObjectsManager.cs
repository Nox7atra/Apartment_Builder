using System;
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
        private IWallObject _SelectedObject;
        #endregion

        #region events

        public event Action OnReset;

        #endregion
        #region properties

        public Mode CurrentMode
        {
            get { return _CurrentMode; }
        }

        public IWallObject SelectedObject
        {
            get { return _SelectedObject; }
        }

        #endregion

        #region public methods

        public void Reset()
        {
            _CurrentMode = Mode.None;
            _SelectedObject = null;
            if (OnReset != null)
                OnReset();
        }
        public void SelectMode(Mode mode)
        {
            _CurrentMode = mode;

        }

        public void SelectObject(IWallObject wallObj)
        {
            _SelectedObject = wallObj;
        }

        
        #endregion


        #region service methods



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

        private const string AssetName = "ObjectsManager.asset";
        public enum Mode
        {
            Doors,
            Windows,
            Vert,
            None
        }
    }
}