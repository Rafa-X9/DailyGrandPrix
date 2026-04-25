namespace DailyGrandPrix.Entities
{
    internal class Track
    {
        public int Id { get; }
        public string Name { get; set; }
        public int StepsPerLap { get; set; }
        public int RaceLaps
        {
            get
            {
                return (int)Math.Ceiling((double)270 / StepsPerLap);
            }
        }

        public Track()
        {
            Name = "Nameless track";
            //this is only while I don't implement json initialization for race
        }

        public Track(int id, string name, int stepsPerLap)
        {
            Id = id;
            Name = name;
            StepsPerLap = stepsPerLap;
        }

        public Track(Dictionary<string, object> json)
        {
            //Id
            if (!json.TryGetValue("Id", out object? id))
            {
                throw new ArgumentException("JSON didn't have an Id key for Track");
            }
            if (!int.TryParse(id.ToString(), out int idInt))
            {
                throw new ArgumentException("JSON's Id value for track wasn't a proper integer");
            }
            Id = idInt;


            //Name
            if (!json.TryGetValue("Name", out object? name) || name is null)
            {
                throw new ArgumentException("JSON didn't have a Name key for Track or it was null");
            }
            string? nameStr = Convert.ToString(name);
            if (string.IsNullOrEmpty(nameStr))
            {
                throw new ArgumentException("JSON's name key for Track was null, empty, or invalid");
            }
            Name = nameStr;


            //Id
            if (!json.TryGetValue("StepsPerLap", out object? steps))
            {
                throw new ArgumentException("JSON didn't have a StepsPerLap key for Track");
            }
            if (!int.TryParse(steps.ToString(), out int stepsInt))
            {
                throw new ArgumentException("JSON's StepsPerLap value for track wasn't a proper integer");
            }
            StepsPerLap = stepsInt;
        }

        public override string ToString()
        {
            return $"{Name.ToUpper()}" +
                $"\nSteps per lap: {StepsPerLap}\n" +
                $"Race laps: {RaceLaps}\n";
        }
    }
}
