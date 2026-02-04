namespace DailyGrandPrix.Entities
{
    internal class Championship
    {
        public int Id { get; private set; }
        public int Year { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public List<Race> Races { get; private set; } = new List<Race>();
    }
}
