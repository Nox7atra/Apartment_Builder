using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Nox7atra.ApartmentEditor
{
    public class ApartmentsManager
    {
        public Apartment CurrentApartment { get; private set; }

        private string _CurrentApartmentName;
        public bool NeedToSave;
    
        public void SaveCurrent()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Apartment));
            StreamWriter writer = new StreamWriter(CreatePath(_CurrentApartmentName));
            ser.Serialize(writer, CurrentApartment);
            writer.Close();
            NeedToSave = false;
        }
        void Load(string name)
        {
            _CurrentApartmentName = name;
            XmlSerializer serializer
                = new XmlSerializer(typeof(Apartment));
            FileStream fs = new FileStream(CreatePath(name), FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            CurrentApartment = (Apartment)serializer.Deserialize(reader);
            fs.Close();
            NeedToSave = false;
        }
        string CreatePath(string name)
        {
            return DataPath + name + ".xml";
        }
        public ApartmentsManager()
        {
            try
            {
                Load("test");
            }
            catch (Exception e)
            {
                CurrentApartment = new Apartment();
            }
        }
       
        const string DataPath = "Assets/ApartmentsEditor/Data/";
    }
}