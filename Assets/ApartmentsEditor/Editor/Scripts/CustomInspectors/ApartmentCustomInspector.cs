using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace Foxsys.ApartmentEditor
{
    [CustomEditor(typeof(Apartment))]
    public class ApartmentCustomInspector : Editor
    {
        private Apartment _ThisApartment;

        private Rect _Dimensions;

        private bool[] _MaterialFoldouts;
        private void OnEnable()
        {
            _ThisApartment = (Apartment) target;
            _Dimensions = _ThisApartment.Dimensions;
            _MaterialFoldouts = new bool[_ThisApartment.RoomsMaterialses.Count];
        }

        public override void OnInspectorGUI()
        {
            TopButtons();
            _ThisApartment.Height = EditorGUILayout.FloatField("Height (cm)", _ThisApartment.Height);

            var dimensions = EditorGUILayout.Vector2Field("Dimensions (cm)", _Dimensions.size).RoundCoordsToInt();
  
            DrawMaterialProperties();
            //_ThisApartment.IsGenerateOutside = EditorGUILayout.Toggle("Generate outside (Directional Light)", _ThisApartment.IsGenerateOutside);
            GenerateButton();

            var dimensionsRect = new Rect(-dimensions.x / 2, -dimensions.y / 2, dimensions.x, dimensions.y);

            _Dimensions = dimensionsRect;

            _ThisApartment.Dimensions = _Dimensions;
        }

        private void DrawMaterialProperties()
        {
            GUILayout.Label("Materials");
            var materials = _ThisApartment.RoomsMaterialses;
            for (int i = 0, count = materials.Count; i < count; i++)
            {
                _MaterialFoldouts[i] = EditorGUILayout.Foldout(_MaterialFoldouts[i], ((Room.Type) i).ToString());
                if (_MaterialFoldouts[i])
                {
                    materials[i].FloorMat =
                        (Material) EditorGUILayout.ObjectField("Floor Material", materials[i].FloorMat,
                            typeof(Material), false);
                    materials[i].RoofMat =
                        (Material) EditorGUILayout.ObjectField("Roof Material", materials[i].RoofMat, typeof(Material),
                            false);
                    materials[i].WallMat =
                        (Material) EditorGUILayout.ObjectField("Wall Material", materials[i].WallMat, typeof(Material),
                            false);
                }

            }
        }
        private void TopButtons()
        {
            GUILayout.BeginHorizontal();
            CreateNewBlueprint();
            OpenBlueprint();
            GUILayout.EndHorizontal();
        }

        private void CreateNewBlueprint()
        {
            if (GUILayout.Button(
                "Create new"
            ))
            {
                var manager = ApartmentsManager.Instance;
                manager.SelectApartment(manager.CreateOrGetApartment("New Apartment" + GUID.Generate()));
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