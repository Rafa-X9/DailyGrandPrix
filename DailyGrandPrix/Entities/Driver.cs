using DailyGrandPrix.Enums;

namespace DailyGrandPrix.Entities
{
    internal class Driver
    {
        //personal info
        public int Id { get; }
        public string Name { get; set; }
        public string Username { get; set; }
        public int Number { get; set; }
        public Teams Team { get; set; }
        public List<DriverRace> Races { get; set; } = [];

        public Driver(Dictionary<string, object> json)
        {
            //Id
            if (!json.TryGetValue("Id", out object? id))
            {
                throw new ArgumentException("JSON didn't have an Id key for Driver");
            }
            if (!int.TryParse(id.ToString(), out int idInt))
            {
                string msg = "\n";
                msg += "Error: JSON's Id key wasn't an integer\n";
                msg += $"Its value was: {id}";
                throw new ArgumentException(msg);
            }
            Id = idInt;
            

            //Name
            if (!json.TryGetValue("Name", out object? name) || name is null)
            {
                throw new ArgumentException("JSON didn't have a Name key for Driver or it was null");
            }
            string? nameStr = Convert.ToString(name);
            if (string.IsNullOrEmpty(nameStr))
            {
                throw new ArgumentException("JSON's name key was null, empty, or invalid");
            }
            Name = nameStr;


            //Username
            if (!json.TryGetValue("Username", out object? username) || username is null)
            {
                throw new ArgumentException("JSON didn't have a Username key for Driver or it was null");
            }
            string? usernameStr = Convert.ToString(username);
            if (string.IsNullOrEmpty(usernameStr))
            {
                throw new ArgumentException("JSON's username key was null, empty, or invalid");
            }
            Username = usernameStr;


            //Number
            if (!json.TryGetValue("Number", out object? number))
            {
                throw new ArgumentException("JSON didn't have a Number key for Driver");
            }
            if (!int.TryParse(number.ToString(), out int numberInt))
            {
                throw new ArgumentException("JSON's Number key wasn't a valid integer");
            }
            Number = numberInt;


            //Team
            if (!json.TryGetValue("Team", out object? team) || team is null || !int.TryParse(team.ToString(), out int teamInt) || !Enum.IsDefined(typeof(Teams), teamInt))
            {
                throw new ArgumentException("JSON didn't have a Team key for Driver or it wasn't a member of Teams enumeration");
            }
            Team = (Teams)teamInt;


            //Races
            if (json.TryGetValue("Races", out object? races) && races is not null && races is List<DriverRace> racesList)
            {
                Races = (List<DriverRace>)races;
            }
        }

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
