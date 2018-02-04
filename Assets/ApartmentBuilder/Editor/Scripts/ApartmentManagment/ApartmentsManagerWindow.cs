using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Foxsys.ApartmentBuilder
{
    public class ApartmentsManagerWindow : EditorWindow
    {
        public static ApartmentsManagerWindow Create()
        {
            var window = GetWindow<ApartmentsManagerWindow>("ApartmentsWindow");
            window.Show();
            return window;
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Refresh"))
            {
                ApartmentsManager.Instance.Refresh();
            }

            GUILayout.Label("Apartments");
            var apartments = ApartmentsManager.Instance.Apartments;

            foreach (var apartment in apartments)
            {
                CreateApartmentLine(apartment);
            }
            GUILayout.EndVertical();
        }

        private void CreateApartmentLine(Apartment apartment)
        {
            GUILayout.BeginHorizontal();
            SelectApartmentButton(apartment);

            GUILayout.EndHorizontal();
        }

        private void SelectApartmentButton(Apartment apartment)
        {
            if (GUILayout.Button(apartment.name))
            {
                ApartmentsManager.Instance.SelectApartment(apartment);
            }
        }

        private void CreateApartmentButton()
        {
            if (GUILayout.Button("Create new apartment"))
            {
                
            }
        }
    }
}