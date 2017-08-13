using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Nox7atra.ApartmentEditor
{
    public class ApartmentsManager
    {
        #region properties
        public Apartment CurrentApartment
        {
            get
            {
                return _CurrentApartment;
            }
        }
     
        #endregion
        #region attributes
        private Apartment _LoadedApartment;

        private Apartment _CurrentApartment;
        private string _CurrentApartmentName;
        public bool NeedToSave;
        #endregion
        #region public methods
        public void Init()
        {
            Load("test");
        }
        public void SaveCurrent()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Apartment));
            StreamWriter writer = new StreamWriter(CreatePath(_CurrentApartmentName));
            ser.Serialize(writer, _CurrentApartment);
            writer.Close();
            NeedToSave = false;
        }
        #endregion

        #region service methods
        void Load(string name)
        {
            _CurrentApartmentName = name;
            XmlSerializer serializer
                = new XmlSerializer(typeof(Apartment));
            FileStream fs = new FileStream(CreatePath(name), FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            _CurrentApartment = (Apartment)serializer.Deserialize(reader);
            fs.Close();
            NeedToSave = false;
        }
        string CreatePath(string name)
        {
            return DATA_PATH + name + ".xml";
        }
        #endregion
        #region constructor
        public ApartmentsManager()
        {
           
        }
        #endregion
        #region constants
        const string DATA_PATH = "Assets/ApartmentsEditor/Data/";
        #endregion
    }
}