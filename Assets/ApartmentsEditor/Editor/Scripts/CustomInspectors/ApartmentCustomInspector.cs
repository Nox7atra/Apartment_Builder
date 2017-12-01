using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentEditor
{
    [CustomEditor(typeof(Apartment))]
    public class ApartmentCustomInspector : Editor
    {
        private Apartment _ThisApartment;

        private Rect _Dimensions;

        private void OnEnable()
        {
            _ThisApartment = (Apartment) target;
            _Dimensions = _ThisApartment.Dimensions;
        }

        public override void OnInspectorGUI()
        {
            var oldName = _ThisApartment.name;
            _ThisApartment.name = EditorGUILayout.TextField(_ThisApartment.name);
            OpenBlueprint();
            _ThisApartment.Height = EditorGUILayout.FloatField("Height (cm)", _ThisApartment.Height);

            var dimensions = EditorGUILayout.Vector2Field("Dimensions (cm)", _Dimensions.size).RoundCoordsToInt();
            _ThisApartment.FloorMaterial =
                (Material)EditorGUILayout.ObjectField("Floor Material", _ThisApartment.FloorMaterial, typeof(Material), false);
            _ThisApartment.WallMaterial =
                (Material) EditorGUILayout.ObjectField("Wall Material", _ThisApartment.WallMaterial, typeof(Material), false);

            _ThisApartment.IsGenerateOutside = EditorGUILayout.Toggle("Generate outside (Directional Light)", _ThisApartment.IsGenerateOutside);
            GenerateButton();

            var dimensionsRect = new Rect(-dimensions.x / 2, -dimensions.y / 2, dimensions.x, dimensions.y);

            if(_ThisApartment.IsApartmentInRect(dimensionsRect))
                _Dimensions = dimensionsRect;

            _ThisApartment.Dimensions = _Dimensions;

            if (oldName != _ThisApartment.name)
            {
                string assetPath = AssetDatabase.GetAssetPath(_ThisApartment.GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, _ThisApartment.name);
                AssetDatabase.SaveAssets();
            }
        }

        private void OpenBlueprint()
        {
            if (GUILayout.Button(
                "Open in Builder"
            ))
            {
                ApartmentsManager.Instance.SelectApartment(_ThisApartment);
                ApartmentEditorWindow.Create();
            }
        }
        private void GenerateButton()
        {
            if (GUILayout.Button(
                "Generate Mesh"
            ))
            {
                MeshBuilder.GenerateApartmentMesh(_ThisApartment);
            }
        }
    }
}