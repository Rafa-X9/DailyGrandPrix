using System.Text.Json.Serialization;

namespace DailyGrandPrix.Entities
{
    internal class Championship
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore] public List<Race> Races { get; set; } = new List<Race>();
        public List<int> RacesIds
        {
            get
            {
                return Races.Select(r => r.Id).ToList();
            }
        }

        public Championship() { }
        public Championship(int id, int year, string name)
        {
            Id = id;
            Year = year;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name.ToUpper()}" +
                $"\nId = {Id}" +
                $"\nHas {Races.Count} races\n";
        }
    }
}
