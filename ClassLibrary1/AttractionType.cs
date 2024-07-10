using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    /// <summary>
    /// Класс с данными об атракционах
    /// </summary>
    [Serializable]
    public class AttractionType
    {
        int _Id;
        string _name;
        string _photo;
        double _global_id;
        string _admArea;
        string _district;
        string _location;
        string _registrationNumber;
        string _state;
        string _locationType;
        string _geodata_center;
        string _geoarea;

        public int Id { get => _Id;}
        public string Name { get => _name;}
        public string Photo { get => _photo;}
        public double global_id { get => _global_id;}
        public string AdmArea { get => _admArea;}
        public string District { get => _district;}
        public string Location { get => _location;}
        public string RegistrationNumber { get => _registrationNumber;}
        public string State { get => _state;}
        public string LocationType { get => _locationType;}
        public string geodata_center { get => _geodata_center;}
        public string geoarea { get => _geoarea;}

        [JsonConstructor]
        public AttractionType(int Id, string Name, string Photo, double global_id, string AdmArea,
                              string District, string Location, string RegistrationNumber, string State,
                              string LocationType, string geodata_center, string geoarea)
        {
            _Id = Id;
            _name = Name;
            _photo = Photo;
            _global_id = global_id;
            _admArea = AdmArea;
            _district = District;
            _location = Location;
            _registrationNumber = RegistrationNumber;
            _state = State;
            _locationType = LocationType;
            _geodata_center = geodata_center;
            _geoarea = geoarea;
        }
    }
}
