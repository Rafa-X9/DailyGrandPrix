using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Entities
{
    internal class Driver
    {
        //personal info
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Number { get; set; }
        public Teams Team { get; set; }
        public List<DriverRace> Races { get; set; } = new List<DriverRace>();

        public Driver() { }

        public Driver(int id, string name, string username,
            int number, Teams team)
        {
            Id = id;
            Name = name;
            Username = username;
            Number = number;
            Team = team;
        }

        public void AddRaces(List<DriverRace> races)
        {
            Races = races;
        }

        public override string ToString()
        {
            return $"{Name.ToUpper()}" +
                $"\nId = {Id}" +
                $"\nUsername = {Username}" +
                $"\nNumber = {Number}" +
                $"\nTeam = {Team}\n";
        }
    }
}
