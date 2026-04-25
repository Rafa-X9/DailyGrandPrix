using System.Text.Json.Serialization;

namespace DailyGrandPrix.Entities
{
    internal class Championship
    {
        public int Id { get; }
        public int Year { get; set; }
        public string Name { get; set; }
        [JsonIgnore] public List<Race> Races { get; set; } = new List<Race>();
        public List<int> RacesIds
        {
            get
            {
                return Races.Select(r => r.Id).ToList();
            }
        }

        public Championship(int id, int year, string name)
        {
            Id = id;
            Year = year;
            Name = name;
        }

        public Championship(Dictionary<string, object> json)
        {
            //Id
            if (!json.TryGetValue("Id", out object? id))
            {
                throw new ArgumentException("JSON didn't have an Id key for Championship");
            }
            if (!int.TryParse(id.ToString(), out int idInt))
            {
                throw new ArgumentException("JSON's Id value for Championship wasn't a proper integer");
            }
            Id = idInt;


            //Year
            if (!json.TryGetValue("Year", out object? year))
            {
                throw new ArgumentException("JSON didn't have a Year key for Championship");
            }
            if (!int.TryParse(year.ToString(), out int yearInt))
            {
                throw new ArgumentException("JSON's Year value for Championship wasn't a proper integer");
            }
            Year = yearInt;


            //Name
            if (!json.TryGetValue("Name", out object? name) || name is null)
            {
                throw new ArgumentException("JSON didn't have a Name key for Championship or it was null");
            }
            string? nameStr = Convert.ToString(name);
            if (string.IsNullOrEmpty(nameStr))
            {
                throw new ArgumentException("JSON's name key for Championship was null, empty, or invalid");
            }
            Name = nameStr;
        }

        public override string ToString()
        {
            return $"{Name.ToUpper()}" +
                $"\nId = {Id}" +
                $"\nHas {Races.Count} races\n";
        }
    }
}
