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

        private void OnEnable()
        {
            _ThisApartment = (Apartment) target;
            _Dimensions = _ThisApartment.Dimensions;

        }

        public override void OnInspectorGUI()
        {
            TopButtons();
            _ThisApartment.Height = EditorGUILayout.FloatField("Height (cm)", _ThisApartment.Height);

            var dimensions = EditorGUILayout.Vector2Field("Dimensions (cm)", _Dimensions.size).RoundCoordsToInt();
            _ThisApartment.PlanImage = (Texture) EditorGUILayout.ObjectField(_ThisApartment.PlanImage, typeof(Texture), false);

            //_ThisApartment.IsGenerateOutside = EditorGUILayout.Toggle("Generate outside (Directional Light)", _ThisApartment.IsGenerateOutside);
            GenerateButton();

            var dimensionsRect = new Rect(-dimensions.x / 2, -dimensions.y / 2, dimensions.x, dimensions.y);

            _Dimensions = dimensionsRect;

            _ThisApartment.Dimensions = _Dimensions;
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