namespace DailyGrandPrix.Entities
{
    internal class Track
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StepsPerLap { get; set; }
        public int RaceLaps { get 
            { return (int)Math.Ceiling((double) 150 / StepsPerLap); } 
        }

        public Track() { }

        public Track(int id, string name, int stepsPerLap)
        {
            Id = id;
            Name = name;
            StepsPerLap = stepsPerLap;
        }

        public override string ToString()
        {
            return $"{Name.ToUpper()}" +
                $"\nSteps per lap: {StepsPerLap}\n";
        }
    }
}
