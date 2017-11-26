using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Foxsys.ApartmentEditor
{
    public class ApartmentsManager
    {
        private Apartment _CurrentApartment;

        private List<Apartment> _Apartments;

        public Apartment CurrentApartment
        {
            get
            {
                if (_CurrentApartment == null)
                {
                    _CurrentApartment = _Apartments.Count > 0 ? _Apartments[0] : Apartment.Create("test");
                }
                return _CurrentApartment;
            }
        }

        public List<Apartment> Apartments
        {
            get { return _Apartments; }
        }

        public void SelectApartment(Apartment apartment)
        {
            _CurrentApartment = apartment;
        }
        private ApartmentsManager()
        {
            Refresh();
        }

        public void Refresh()
        {
            string[] assets = Directory.GetFiles(PathsConfig.Instance.PathToApartments, "*.asset");
            _Apartments = new List<Apartment>(assets.Length);
            foreach (var asset in assets)
            {
                var apartment = AssetDatabase.LoadAssetAtPath<Apartment>(asset);
                if (apartment)
                    _Apartments.Add(apartment);
            }
            _CurrentApartment = null;
        }
        private static ApartmentsManager _Instance;
        public static ApartmentsManager Instance
        {
            get { return _Instance ?? (_Instance = new ApartmentsManager()); }
        }
    }
}