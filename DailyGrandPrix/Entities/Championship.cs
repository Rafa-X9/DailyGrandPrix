namespace DailyGrandPrix.Entities
{
    internal class Championship
    {
        public int Id { get; private set; }
        public int Year { get; private set; }
        public string Name { get; set; } = string.Empty;
        public List<Race> Races { get; private set; } = new List<Race>();

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
