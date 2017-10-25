
using UnityEditor;
using UnityEngine;
namespace Nox7atra.ApartmentEditor
{
    public class SkinManager
    {
        private const string DefaultSkinName = "Default.asset";
        private const string SkinsPath = "Assets/Editor/ApartmentsEditor/Skins/";
        public Skin CurrentSkin { get; private set; }

        private SkinManager()
        {
            string path = SkinsPath + DefaultSkinName;
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