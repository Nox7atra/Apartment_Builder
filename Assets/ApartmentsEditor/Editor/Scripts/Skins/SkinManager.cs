
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Foxsys.ApartmentEditor
{
    public class SkinManager
    {
        private const string ProSkin = "Pro.asset";
        public Skin CurrentSkin { get; private set; }

        private SkinManager()
        {
            string path = Path.Combine(PathsConfig.Instance.PathToSkins, ProSkin);
            CurrentSkin = AssetDatabase.LoadAssetAtPath<Skin>(path);
            if (CurrentSkin == null)
            {
                CurrentSkin = ScriptableObject.CreateInstance<Skin>();
                AssetDatabase.CreateAsset(CurrentSkin, path);
            }
        }
        public static SkinManager Instance
        {
            get { return _Instance ?? (_Instance = new SkinManager()); }
        }
        private static SkinManager _Instance;
    }
}